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

[<Property(Replay = "(1909695862,296706715)")>]
let ``Parsing strings: System.Text.Json`` (input : char[]) =
    let expected = new string(input)
    let actual = JsonDocument.Parse (toJsonLiteralString input)
    Assert.Equal(expected, actual.RootElement.GetString())
    
[<Property>]
let ``Parsing strings: Newtonsoft.Json`` (input : char[]) =
    let expected = new string(input)
    let actual = Newtonsoft.Json.JsonConvert.DeserializeObject<string> (toJsonLiteralString input)
    Assert.Equal(expected, actual)