

using System;
using System.Collections.Generic;
using System.IO;

public static class Reservados
{

    // ── Tabla maestra: reservadas + INDENT en índice 80 ───────
    public static readonly List<string> Palabras = ConstruirPalabras();

    static List<string> ConstruirPalabras()
{
    var lista = new List<string>
    {
        // palabras
        "print", "if", "else", "while", "for", "return", "this",
        "break", "continue",
        "void", "switch", "case", "default", "throw",
        "true", "false", "null",
        "enum",

        // tipos enteros
        "uint8", "uint16", "uint32", "uint64",
        "int8", "int16", "int32", "int64",

        // tipos string
        "string8", "string16", "string32", "string64",
        "string128", "string256", "string512", "string1024",
        "string",

        // otros tipos
        "bool", "float", "double",

        // operadores multi-char
        "==", "!=", "<=", ">=", "&&", "||", "->", "::",
        "++", "--", "+=", "-=", "*=", "/=", "%=",
        "<<", ">>", "**",

        // operadores / símbolos de 1 char
        "=", "+", "-", "*", "/", "%", "<", ">", "!",
        "&", "|", "^", "~", "?",
        "(", ")", "{", "}", "[", "]",
        ";", ",", ".", ":",

        // token léxico de indentación (índice 80)
        "INDENT", "data", "\n",

        // ── Literales numéricos 1..64 (índices 84..147) ──────
        "0","1", "2", "3", "4", "5", "6", "7", "8",
        "9", "10", "11", "12", "13", "14", "15", "16",
        "17", "18", "19", "20", "21", "22", "23", "24",
        "25", "26", "27", "28", "29", "30", "31", "32",
        "33", "34", "35", "36", "37", "38", "39", "40",
        "41", "42", "43", "44", "45", "46", "47", "48",
        "49", "50", "51", "52", "53", "54", "55", "56",
        "57", "58", "59", "60", "61", "62", "63", "64"
        , "struct","char"
    };
    return lista;
}

    
    // ── Palabras clave de control ──────────────────────────────
    public const int _print = 0;
    public const int _if = 1;
    public const int _else = 2;
    public const int _while = 3;
    public const int _for = 4;
    public const int _return = 5;
    public const int _this = 6;
    public const int _break = 7;
    public const int _continue = 8;
    public const int _void = 9;
    public const int _switch = 10;
    public const int _case = 11;
    public const int _default = 12;
    public const int _throw = 13;
    public const int _true = 14;
    public const int _false = 15;
    public const int _null = 16;
    public const int _enum = 17;

    // ── Tipos enteros ──────────────────────────────────────────
    public const int _uint8 = 18;
    public const int _uint16 = 19;
    public const int _uint32 = 20;
    public const int _uint64 = 21;
    public const int _int8 = 22;
    public const int _int16 = 23;
    public const int _int32 = 24;
    public const int _int64 = 25;

    // ── Tipos string ───────────────────────────────────────────
    public const int _string8 = 26;
    public const int _string16 = 27;
    public const int _string32 = 28;
    public const int _string64 = 29;
    public const int _string128 = 30;
    public const int _string256 = 31;
    public const int _string512 = 32;
    public const int _string1024 = 33;
    public const int _string = 34;

    // ── Otros tipos ────────────────────────────────────────────
    public const int _bool = 35;
    public const int _float = 36;
    public const int _double = 37;

    // ── Operadores multi-char ──────────────────────────────────
    public const int _eq = 38;
    public const int _neq = 39;
    public const int _le = 40;
    public const int _ge = 41;
    public const int _andand = 42;
    public const int _oror = 43;
    public const int _arrow = 44;
    public const int _coloncolon = 45;
    public const int _inc = 46;
    public const int _dec = 47;
    public const int _addeq = 48;
    public const int _subeq = 49;
    public const int _muleq = 50;
    public const int _diveq = 51;
    public const int _modeq = 52;
    public const int _shl = 53;
    public const int _shr = 54;
    public const int _pow = 55;

    // ── Operadores / símbolos de 1 char ────────────────────────
    public const int _assign = 56;
    public const int _plus = 57;
    public const int _minus = 58;
    public const int _star = 59;
    public const int _slash = 60;
    public const int _percent = 61;
    public const int _lt = 62;
    public const int _gt = 63;
    public const int _not = 64;
    public const int _amp = 65;
    public const int _pipe = 66;
    public const int _caret = 67;
    public const int _tilde = 68;
    public const int _question = 69;
    public const int _lparen = 70;
    public const int _rparen = 71;
    public const int _lbrace = 72;
    public const int _rbrace = 73;
    public const int _lbracket = 74;
    public const int _rbracket = 75;
    public const int _semicolon = 76;
    public const int _comma = 77;
    public const int _dot = 78;
    public const int _colon = 79;
    public const int _indent = 80;
    public const int _data = 81;
    public const int _saltoLinea = 82;

    
// ── Literales numéricos 1..64 (índices 84..147) ────────────
    public const int _num0  = 83;
    public const int _num1  = 84;
    public const int _num2  = 85;
    public const int _num3  = 86;
    public const int _num4  = 87;
    public const int _num5  = 88;
    public const int _num6  = 89;
    public const int _num7  = 90;
    public const int _num8  = 91;
    public const int _num9  = 92;
    public const int _num10 = 93;
    public const int _num11 = 94;
    public const int _num12 = 95;
    public const int _num13 = 96;
    public const int _num14 = 97;
    public const int _num15 = 98;
    public const int _num16 = 99;
    public const int _num17 = 100;
    public const int _num18 = 101;
    public const int _num19 = 102;
    public const int _num20 = 103;
    public const int _num21 = 104;
    public const int _num22 = 105;
    public const int _num23 = 106;
    public const int _num24 = 107;
    public const int _num25 = 108;
    public const int _num26 = 109;
    public const int _num27 = 110;
    public const int _num28 = 111;
    public const int _num29 = 112;
    public const int _num30 = 113;
    public const int _num31 = 114;
    public const int _num32 = 115;
    public const int _num33 = 116;
    public const int _num34 = 117;
    public const int _num35 = 118;
    public const int _num36 = 119;
    public const int _num37 = 120;
    public const int _num38 = 121;
    public const int _num39 = 122;
    public const int _num40 = 123;
    public const int _num41 = 124;
    public const int _num42 = 125;
    public const int _num43 = 126;
    public const int _num44 = 127;
    public const int _num45 = 128;
    public const int _num46 = 129;
    public const int _num47 = 130;
    public const int _num48 = 131;
    public const int _num49 = 132;
    public const int _num50 = 133;
    public const int _num51 = 134;
    public const int _num52 = 135;
    public const int _num53 = 136;
    public const int _num54 = 137;
    public const int _num55 = 138;
    public const int _num56 = 139;
    public const int _num57 = 140;
    public const int _num58 = 141;
    public const int _num59 = 142;
    public const int _num60 = 143;
    public const int _num61 = 144;
    public const int _num62 = 145;
    public const int _num63 = 146;
    public const int _num64 = 147;

    public const int _struct = 148;
    public const int _char = 149;
    // ── Índice reservado (solo reservadas, sin dinámicas) ─────
    static readonly Dictionary<string, int> _mapa = ConstruirMapa();

    static Dictionary<string, int> ConstruirMapa()
    {
        var m = new Dictionary<string, int>(Palabras.Count);
        for (int i = 0; i < Palabras.Count; i++)
            m[Palabras[i]] = i;
        return m;
    }

    public static int BaseDinamicas => Palabras.Count; // = 81

    // ── Consultas ─────────────────────────────────────────────
    public static int IndiceDe(string palabra)
        => palabra is not null && _mapa.TryGetValue(palabra, out int i) ? i : -1;

    public static bool EsReservada(string palabra)
        => palabra is not null && _mapa.ContainsKey(palabra);


    public static void GuardarDiccionario(string path, IEnumerable<string> dinamicas)
    {
        var vistas = new HashSet<string>(StringComparer.Ordinal);
        using var sw = new StreamWriter(path, false);

        foreach (var d in dinamicas)
        {
            if (string.IsNullOrEmpty(d)) continue;       // sin huecos
            if (EsReservada(d)) continue;                // sin reservadas
            if (!vistas.Add(d)) continue;                // sin duplicados

            sw.WriteLine(d);
        }
    }

    // ── Diccionario dinámico: CARGAR garantizando reservadas ──
    /// <summary>
    /// Carga las dinámicas desde disco garantizando que:
    ///   - solo contenga palabras NO reservadas,
    ///   - sin vacías, sin duplicados,
    ///   - el orden preserva el que había en disco (para que los índices
    ///     guardados en .mc sigan siendo válidos).
    /// Las reservadas NO se cargan: siempre viven en Palabras.
    /// </summary>
    public static List<string> CargarDiccionario(string path)
    {
        var lista = new List<string>();
        if (string.IsNullOrEmpty(path) || !File.Exists(path)) return lista;

        var vistas = new HashSet<string>(StringComparer.Ordinal);

        foreach (var raw in File.ReadAllLines(path))
        {
            if (string.IsNullOrEmpty(raw)) continue;   // ignora huecos antiguos
            if (EsReservada(raw)) continue;            // ignora reservadas coladas
            if (!vistas.Add(raw)) continue;            // ignora duplicados

            lista.Add(raw);
        }

        return lista;
    }

    // ── Traducción índice → texto ─────────────────────────────
    /// <summary>
    /// Traduce cualquier índice a texto.
    ///   - índices 0..80    → siempre Palabras (reservadas + INDENT)
    ///   - índices ≥ 81     → dinamicas[indice - 81]
    /// Si el índice cae fuera de rango, devuelve un marcador diagnóstico.
    /// </summary>
    public static string TextoDe(int indice, IReadOnlyList<string> dinamicas)
    {
        if (indice < 0) return $"<{indice}>";
        if (indice < Palabras.Count) return Palabras[indice];

        int dyn = indice - Palabras.Count;
        if (dinamicas is not null && dyn < dinamicas.Count)
            return dinamicas[dyn];

        return $"<{indice}>";
    }
}