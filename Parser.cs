using System;
using System.Collections.Generic;

public class Parser
{
    readonly List<int> _indices;

    public List<int> enums = new();
    public List<int> datas = new();   // renombrado: data -> datas
    public List<int> structs = new();
    public List<int> resto = new();   // renombrado: Resto -> resto

    public Parser(byte[] bytes)
    {
        if (bytes == null) throw new ArgumentNullException(nameof(bytes));
        _indices = new List<int>();
        foreach (uint v in Tokenizador.LeerVarints(bytes))
            _indices.Add(checked((int)v));
    }

    public void Procesar()
    { // simulando que leemos token por token
        int index = 0;
        while (index < _indices.Count)
        {
            ProcesarToken(_indices[index]);
            index++;
        }
        ProcesarToken(-1); // finalizar token;
        ProcesarEnums();
    }



    int _indent = 0;
    List<int> _line = new();
    void ProcesarToken(int token)
    {
        _line.Add(token);
        if (token == Reservados._saltoLinea)
        {
            ProcesarLine(_line, _indent);
            _indent = 0;
            _line.Clear();
        }
        else if (token == Reservados._indent)
        {
            _indent++;
        }
        else if (token == -1)
        {
            _line.RemoveAt(_line.Count - 1);
            ProcesarLine(_line, _indent);
            _indent = 0;
            _line.Clear();
        }
    }

    int _indent_guardado = 0;
    List<int> _ls = null;
    void ProcesarLine(List<int> line, int indent)
    {
        if (line.Count < 2) return; // solo salto de linea;
        if (line.Count <= indent) return; // no hay nada mas que indents
        if (_ls == null)
        {
            if (line[indent] == Reservados._struct) _ls = structs;
            else if (line[indent] == Reservados._enum) _ls = enums;
            else if (line[indent] == Reservados._data) _ls = datas;

            if (_ls != null)
            {
                _ls.AddRange(line);
                _indent_guardado = indent;
            }
            else resto.AddRange(line);
        }
        else
        {
            if (indent <= _indent_guardado)
            {
                // salir
                _ls = null;
                _indent_guardado = 0;
                ProcesarLine(line, indent);
            }
            else
            {
                _ls.AddRange(line);
            }
        }

    }



    void ProcesarEnums()
    {
        List<int> nombres = new();
        int index = 0;
        while (index < enums.Count)
        {
            // Esperamos: _enum, nombre, _saltoLinea
            if (enums[index] != Reservados._enum)
            {
                index++;
                continue;
            }

            index++; // saltamos _enum
            if (index >= enums.Count) break;

            int nombre = enums[index];
            index++; // saltamos nombre

            // saltamos el _saltoLinea de la cabecera
            while (index < enums.Count && enums[index] == Reservados._saltoLinea)
                index++;

            // Recogemos los valores (cada línea empieza con _indent)
            var valores = new List<int>();
            while (index < enums.Count)
            {
                if (enums[index] == Reservados._indent)
                {
                    index++; // saltamos _indent
                    if (index >= enums.Count) break;
                    valores.Add(enums[index]); // el identificador del valor
                    index++;
                    // saltamos posibles _saltoLinea
                    while (index < enums.Count && enums[index] == Reservados._saltoLinea)
                        index++;
                }
                else if (enums[index] == Reservados._saltoLinea)
                {
                    index++;
                }
                else
                {
                    // encontramos el inicio de otro bloque (otro _enum, o _struct, etc.)
                    break;
                }
            }

            ProcesarEnum(nombre, valores.ToArray());
            nombres.Add(nombre);
        }

        foreach (var item in nombres)
        {
            // hacer foreach en structs y en resto y global donde... se cambiara item por el valor de Reservados._int8 si no le sigue un 

            Intercambiar(structs, item, Reservados._int8);
            Intercambiar(datas, item, Reservados._int8);
            Intercambiar(resto, item, Reservados._int8);
        }
    }
    void Intercambiar(List<int> ints, int de, int a)
    {
        for (int i = 0; i < ints.Count; i++)
        {
            int item = ints[i];
            if (item == de) ints[i] = a;
        }
    }
    void ProcesarEnum(int nombre, int[] valores)
    {
        // Aquí decides qué hacer con el enum.
        // Por ejemplo, guardarlo en un diccionario o emitir código.
        List<int> inyectar = new() { Reservados._saltoLinea, Reservados._data, nombre };
        for (int i = 0; i < valores.Length; i++)
        {
            inyectar.AddRange([Reservados._saltoLinea, Reservados._indent, valores[i], Reservados._assign, Reservados._num0 + i]);
        }
        datas.AddRange(inyectar);
    }




}

public class Funciones
{
    public List<int> lista;
    int indent_contado = -1;
    void ProcesarFuncionesResto(List<int> line, int indent)
    {
        if (indent_contado == -1)
        {
            // verificando
            
        }
        else
        {
            // extrayendo
        }
    }  
}
