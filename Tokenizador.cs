using System;
using System.Collections;
using System.IO;
using System.Text;

public sealed class Tokenizador : IDisposable
{
    const int TAM_BUFFER = 4096;

    // ── Estado de instancia ─────────────────────────────────
    readonly string[] _palabras;
    readonly int _indentIdx;
    readonly int _baseDinamicas;

    readonly Hashtable _reservadasIdx;   // string -> int (reservadas)
    readonly Hashtable _dinamicasIdx;    // string -> int (dinámicas)

    string[] _operadores;                // ordenados por longitud desc
    int _operadoresCount;

    string[] _dinamicas;                 // solo texto, índice = _baseDinamicas + i
    int _dinamicasCount;

    // Buffer reutilizable para literales de string (evita StringBuilder por token)
    char[] _litBuf;
    int _litLen;

    // ── Recursos ────────────────────────────────────────────
    FileStream _fsOut;
    BufferedStream _bsOut;
    StreamWriter _dicWriter;
    bool _disposed;
    bool _dicEscrito;

    // ── Estadísticas ────────────────────────────────────────
    long _tamOriginalTotal;
    long _tamMc;
    long _tamDic;
    int _numTokensTotal;
    int _archivosProcesados;

    public int NumTokens { get { return _numTokensTotal; } }
    public int ArchivosProcesados { get { return _archivosProcesados; } }
    public int Dinamicas { get { return _dinamicasCount; } }
    public long TamOriginal { get { return _tamOriginalTotal; } }
    public long TamMc { get { return _tamMc; } }
    public long TamDic { get { return _tamDic; } }

    public Tokenizador(string output, string diccionario)
    {
        if (output == null) throw new ArgumentNullException("output");
        if (diccionario == null) throw new ArgumentNullException("diccionario");

        _palabras = Reservados.Palabras.ToArray();
        _indentIdx = Reservados._indent;
        _baseDinamicas = Reservados.BaseDinamicas;

        // Índice de reservadas
        _reservadasIdx = new Hashtable(_palabras.Length);
        for (int i = 0; i < _palabras.Length; i++)
            _reservadasIdx[_palabras[i]] = i;

        _dinamicasIdx = new Hashtable(64);

        ConstruirOperadores();

        _dinamicas = new string[64];
        _dinamicasCount = 0;

        _litBuf = new char[64];
        _litLen = 0;

        _fsOut = new FileStream(output, FileMode.Create, FileAccess.Write);
        _bsOut = new BufferedStream(_fsOut, TAM_BUFFER);

        _dicWriter = new StreamWriter(
            new FileStream(diccionario, FileMode.Create, FileAccess.Write));
    }

    public int Ejecutar(string[] inputs)
    {
        if (_disposed) throw new ObjectDisposedException("Tokenizador");
        if (inputs == null || inputs.Length == 0) return 0;

        int tokensInicio = _numTokensTotal;

        for (int a = 0; a < inputs.Length; a++)
        {
            string rutaIn = inputs[a];
            if (rutaIn == null) continue;
            ProcesarArchivo(rutaIn);
            _archivosProcesados++;
        }

        _bsOut.Flush();
        EscribirDiccionario();

        _fsOut.Flush();
        _tamMc = _fsOut.Length;
        _tamDic = _dicWriter.BaseStream.Length;

        return _numTokensTotal - tokensInicio;
    }

    void EscribirDiccionario()
    {
        if (_dicEscrito) return;
        _dicEscrito = true;
        for (int i = 0; i < _dinamicasCount; i++)
            _dicWriter.WriteLine(_dinamicas[i]);
        _dicWriter.Flush();
    }

    void ProcesarArchivo(string rutaIn)
    {
        using (var fsIn = new FileStream(rutaIn, FileMode.Open, FileAccess.Read))
        using (var reader = new StreamReader(fsIn))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                _tamOriginalTotal += (line.Length + 1);
                Tokenizar(line);

                // FIX: emitir token de fin de línea lógico (\n).
                // ReadLine() consume el terminador, así que lo emitimos aquí.
                EscribirVarint(_bsOut, (uint)Reservados._saltoLinea);
                _numTokensTotal++;
            }
        }
    }

    // ── Operadores ──────────────────────────────────────────
    void ConstruirOperadores()
    {
        string indentTok = _palabras[_indentIdx];
        string saltoTok = _palabras[Reservados._saltoLinea]; // FIX: excluir "\n"

        int n = 0;
        for (int i = 0; i < _palabras.Length; i++)
        {
            string p = _palabras[i];
            if (p == indentTok) continue;
            if (p == saltoTok) continue;  // FIX: "\n" no es operador léxico
            if (EsSimbolo(p)) n++;
        }

        _operadores = new string[n + 16]; // holgura para dinámicas
        int k = 0;
        for (int i = 0; i < _palabras.Length; i++)
        {
            string p = _palabras[i];
            if (p == indentTok) continue;
            if (p == saltoTok) continue;  // FIX
            if (EsSimbolo(p)) _operadores[k++] = p;
        }
        _operadoresCount = k;

        OrdenarOperadores();
    }

    void OrdenarOperadores()
    {
        for (int i = 1; i < _operadoresCount; i++)
        {
            string cur = _operadores[i];
            int j = i - 1;
            while (j >= 0 && _operadores[j].Length < cur.Length)
            {
                _operadores[j + 1] = _operadores[j];
                j--;
            }
            _operadores[j + 1] = cur;
        }
    }

    void AgregarOperador(string op)
    {
        // FIX: nunca agregar "\n" (ni vacío) como operador dinámico.
        if (string.IsNullOrEmpty(op)) return;
        if (op == _palabras[Reservados._saltoLinea]) return;

        if (_operadoresCount == _operadores.Length)
        {
            string[] nuevo = new string[_operadores.Length * 2];
            Array.Copy(_operadores, nuevo, _operadoresCount);
            _operadores = nuevo;
        }
        _operadores[_operadoresCount++] = op;
        OrdenarOperadores();
    }

    // ── Índices ─────────────────────────────────────────────
    int IndiceDinamica(string texto)
    {
        object o = _dinamicasIdx[texto];
        if (o != null) return (int)o;

        // Nueva dinámica
        if (_dinamicasCount == _dinamicas.Length)
        {
            string[] nd = new string[_dinamicas.Length * 2];
            Array.Copy(_dinamicas, nd, _dinamicasCount);
            _dinamicas = nd;
        }
        int idx = _baseDinamicas + _dinamicasCount;
        _dinamicas[_dinamicasCount++] = texto;
        _dinamicasIdx[texto] = idx;

        if (EsSimbolo(texto)) AgregarOperador(texto);

        return idx;
    }

    // ── varint ──────────────────────────────────────────────
    static void EscribirVarint(BufferedStream dst, uint valor)
    {
        while (valor >= 0x80)
        {
            dst.WriteByte((byte)((valor & 0x7F) | 0x80));
            valor >>= 7;
        }
        dst.WriteByte((byte)valor);
    }

    // ── Emisión con índice directo (camino caliente) ────────
    // Busca en reservadas por rango [start, len). Si no está, materializa
    // solo si hace falta (dinámica nueva). Evita Substring en el caso común.
    void EmitirRango(string s, int start, int len)
    {
        // FIX: no comparar contra INDENT / "\n" (no son identificadores válidos).
        int indentIdx = _indentIdx;
        int saltoLinea = Reservados._saltoLinea;

        for (int i = 0; i < _palabras.Length; i++)
        {
            if (i == indentIdx || i == saltoLinea) continue;

            string p = _palabras[i];
            if (p.Length != len) continue;
            bool ok = true;
            for (int t = 0; t < len; t++)
                if (p[t] != s[start + t]) { ok = false; break; }
            if (ok)
            {
                EscribirVarint(_bsOut, (uint)i);
                _numTokensTotal++;
                return;
            }
        }

        // Dinámica: hay que materializar para Hashtable
        string tok = s.Substring(start, len);
        int idx = IndiceDinamica(tok);
        EscribirVarint(_bsOut, (uint)idx);
        _numTokensTotal++;
    }

    // ── Lexer ───────────────────────────────────────────────
    void Tokenizar(string s)
    {
        if (s == null) return;
        int n = s.Length;
        if (n == 0) return;

        int p0 = 0;
        while (p0 < n && s[p0] == ' ') p0++;
        if (p0 + 1 < n && s[p0] == '/' && s[p0 + 1] == '/') return;

        int niveles = p0 / 4;
        for (int k = 0; k < niveles; k++)
        {
            EscribirVarint(_bsOut, (uint)_indentIdx);
            _numTokensTotal++;
        }

        int i = p0;

        while (i < n)
        {
            char c = s[i];

            if (c == ' ' || c == '\t') { i++; continue; }
            if (c == '/' && i + 1 < n && s[i + 1] == '/') break;

            // String literal → buffer reutilizable
            if (c == '"')
            {
                _litLen = 0;
                i++;
                while (i < n && s[i] != '"')
                {
                    if (s[i] == '\\' && i + 1 < n)
                    {
                        char e = s[i + 1];
                        char outC;
                        if (e == 'n') outC = '\n';
                        else if (e == 't') outC = '\t';
                        else if (e == 'r') outC = '\r';
                        else if (e == '\\') outC = '\\';
                        else if (e == '"') outC = '"';
                        else outC = e;
                        PushLit(outC);
                        i += 2;
                    }
                    else PushLit(s[i++]);
                }
                if (i < n) i++;
                EmitirLiteral();
                continue;
            }

            // Número
            if (EsDigito(c))
            {
                int start = i;
                while (i < n && EsDigito(s[i])) i++;
                EmitirRango(s, start, i - start);
                continue;
            }

            // Identificador
            if (EsLetra(c))
            {
                int start = i;
                while (i < n && (EsLetra(s[i]) || EsDigito(s[i]))) i++;
                EmitirRango(s, start, i - start);
                continue;
            }

            // Operador (match más largo)
            string match = null;
            for (int k = 0; k < _operadoresCount; k++)
            {
                string op = _operadores[k];
                if (i + op.Length > n) continue;
                bool ok = true;
                for (int t = 0; t < op.Length; t++)
                    if (s[i + t] != op[t]) { ok = false; break; }
                if (ok) { match = op; break; }
            }
            if (match != null)
            {
                // Operador: buscar índice (reservada o dinámica)
                object ro = _reservadasIdx[match];
                int idx;
                if (ro != null) idx = (int)ro;
                else
                {
                    object dobj = _dinamicasIdx[match];
                    if (dobj != null) idx = (int)dobj;
                    else idx = IndiceDinamica(match);
                }
                EscribirVarint(_bsOut, (uint)idx);
                _numTokensTotal++;
                i += match.Length;
                continue;
            }

            // Carácter suelto
            EmitirCharSuelto(c);
            i++;
        }
    }

    void PushLit(char ch)
    {
        if (_litLen == _litBuf.Length)
        {
            char[] nb = new char[_litBuf.Length * 2];
            Array.Copy(_litBuf, nb, _litLen);
            _litBuf = nb;
        }
        _litBuf[_litLen++] = ch;
    }

    void EmitirLiteral()
    {
        string tok = new string(_litBuf, 0, _litLen);
        int idx = IndiceDinamica(tok);
        EscribirVarint(_bsOut, (uint)idx);
        _numTokensTotal++;
    }

    void EmitirCharSuelto(char c)
    {
        string tok = c.ToString();
        object ro = _reservadasIdx[tok];
        int idx;
        if (ro != null) idx = (int)ro;
        else
        {
            object dobj = _dinamicasIdx[tok];
            if (dobj != null) idx = (int)dobj;
            else idx = IndiceDinamica(tok);
        }
        EscribirVarint(_bsOut, (uint)idx);
        _numTokensTotal++;
    }

    // ── Utilidades ──────────────────────────────────────────
    static bool EsLetra(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
    }
    static bool EsDigito(char c)
    {
        return c >= '0' && c <= '9';
    }
    static bool EsSimbolo(string s)
    {
        if (s == null || s.Length == 0) return false;
        if (s == "_._._._") return true;
        for (int i = 0; i < s.Length; i++)
            if (EsLetra(s[i]) || EsDigito(s[i])) return false;
        return true;
    }

    // ── Dispose ─────────────────────────────────────────────
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        try
        {
            if (_dicWriter != null)
            {
                EscribirDiccionario();
                _dicWriter.Dispose();
                _dicWriter = null;
            }
        }
        catch { }

        try { if (_bsOut != null) { _bsOut.Flush(); _bsOut.Dispose(); _bsOut = null; } } catch { }
        try { if (_fsOut != null) { _fsOut.Dispose(); _fsOut = null; } } catch { }

        _dinamicas = null;
        _operadores = null;
        _litBuf = null;
    }
    
    public static IEnumerable<int> LeerVarints(byte[] data)
    {
        int valor = 0, shift = 0;
        foreach (byte b in data)
        {
            valor |= (b & 0x7F) << shift;
            if ((b & 0x80) == 0)
            {
                yield return valor;
                valor = 0;
                shift = 0;
            }
            else shift += 7;
        }
        if (shift != 0) throw new InvalidDataException("Varint truncado al final.");
    }
}