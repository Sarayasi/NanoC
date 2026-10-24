using System;
using System.IO;
using System.Linq;

// ── 1) Tokenizar ────────────────────────────────────────────
using (var token = new Tokenizador("binario", "diccionario.txt"))
{
    token.Ejecutar(new[] { "input/Test.nanoc" });   // new[] para C# ≤ 11
}

// ── 2) Leer binario y diccionario UNA sola vez ──────────────
byte[] mcBytes   = File.ReadAllBytes("binario");
var    dinamicas = Reservados.CargarDiccionario("diccionario.txt");

// ── 3) Reconstruir ──────────────────────────────────────────
// LeerVarints devuelve IEnumerable<uint>; Reconstruir acepta IEnumerable<uint>.
File.WriteAllText(
    "reconstruccion.txt",
    Reconstructor.Reconstruir(Tokenizador.LeerVarints(mcBytes), dinamicas));

// ── 4) Volcado legible de índices (debug) ───────────────────                 // funciona con uint[]
File.WriteAllText("binario.txt", bytesToIndices(mcBytes));

// ── 5) Parser y reconstrucciones parciales ──────────────────
var parser = new Parser(mcBytes);
parser.Procesar();
//
File.WriteAllText("_structs.txt",
    inttostring(parser.structs.ToArray()));
File.WriteAllText("_enums.txt",
    inttostring(parser.enums.ToArray()));
File.WriteAllText("_resto.txt",
    inttostring(parser.resto.ToArray()));
File.WriteAllText("_data.txt",
    inttostring(parser.datas.ToArray()));


string bytesToIndices(byte[] bytes)
{
    var ls = Tokenizador.LeerVarints(mcBytes).ToArray();
    string line = string.Join(" ", ls);                 // funciona con uint[]
    return line;
}
string intToIndices(int[] bytes)
{

    string line = string.Join(" ", bytes);                 // funciona con uint[]
    return line;
}
string inttostring(int[] ints) => Reconstructor.Reconstruir(ints, dinamicas);