module System.Text.Json.Tests

open Xunit
open FsCheck.Xunit

let toJsonLiteralString (input : char[]) =
    let sb = System.Text.StringBuilder()
    let _ = sb.Append("\"")
    for c in input do
        match c with
        | '\\' -> sb.Append('\\').Append('\\') |> ignore
        | '"'  -> sb.Append('\\').Append('"') |> ignore
        | c    -> sb.Append(c) |> ignore

    let _ = sb.Append("\"")
    sb.ToString()

// Passes all runs
[<Property>]
let ``Parsing strings: Newtonsoft.Json`` (input : char[]) =
    let expected = new string(input)
    let actual = Newtonsoft.Json.JsonConvert.DeserializeObject<string> (toJsonLiteralString input)
    Assert.Equal(expected, actual)

// fails with 
// Falsifiable, after 5 tests (5 shrinks) (StdGen (1909695862,296706715)):
// Original:
// [| '\024'; '"'; 'g'; 'z'; '>'; ','|]
// Shrunk:
// [|'\024'|]
// 
// ---- System.Text.Json.JsonReaderException : '0x18' is invalid within a JSON string. The string should be correctly escaped. LineNumber: 0 | BytePositionInLine: 1.
[<Property(Replay = "(1909695862,296706715)")>]
let ``Parsing strings: System.Text.Json`` (input : char[]) =
    let expected = new string(input)
    let actual = JsonDocument.Parse (toJsonLiteralString input)
    Assert.Equal(expected, actual.RootElement.GetString())