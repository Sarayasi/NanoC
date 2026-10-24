using System;
using System.Collections.Generic;
using System.Text;

public static class Reconstructor
{
    /// <summary>
    /// Reconstruye código fuente a partir de una secuencia de tokens.
    /// </summary>
    /// <param name="tokens">Índices emitidos por el Tokenizador.</param>
    /// <param name="dinamicas">Lista de dinámicas en el mismo orden que el diccionario.</param>
    /// <param name="indentSpaces">Espacios por nivel de indentación (el tokenizador usa 4).</param>
    public static string Reconstruir(
        IEnumerable<int> tokens,
        IReadOnlyList<string> dinamicas,
        int indentSpaces = 4)
    {
        if (tokens == null) throw new ArgumentNullException(nameof(tokens));

        var sb = new StringBuilder();

        foreach (int idx in tokens)
        {

            // ── Indentación ────────────────────────────────────
            if (idx == Reservados._indent)
            {
                sb.Append("    ");
            }
            else if (idx == Reservados._saltoLinea)
            {
                sb.AppendLine();
            }
            else sb.Append(Reservados.TextoDe(idx, dinamicas) + " ");
        }

        return sb.ToString();
    }

    static bool EsSoloSimbolos(string s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
                (c >= '0' && c <= '9') || c == '_')
                return false;
        }
        return true;
    }

}