# Third-party assets and licences

Everything Beyond Nations ships that was not written for it, and the terms it
arrives under.

## Fonts

### DejaVu Sans

| | |
| --- | --- |
| File | `src/BeyondNations.Desktop/assets/fonts/DejaVuSans.ttf` |
| Licence | `src/BeyondNations.Desktop/assets/fonts/LICENSE-DejaVu.txt` |
| Upstream | <https://dejavu-fonts.org/> |
| Terms | Bitstream Vera licence; DejaVu's own changes are public domain |

Used for world-space nametags above pawns and settlements, packed into a glyph
atlas once at startup.

A font had to be bundled because the Unity build drew its labels with
`LegacyRuntime.ttf`, a font that ships inside the Unity editor and disappears
along with it. See #222.

DejaVu Sans was chosen because its licence permits redistribution and
modification with only an attribution and a non-endorsement condition, it
carries wide glyph coverage, and it is a stable, long-maintained face rather
than one that might vanish.

The licence requires that the copyright notice travels with the font, which is
why `LICENSE-DejaVu.txt` sits beside the `.ttf` and is copied to the build
output rather than living only here.

## Packages

Restored from nuget.org at build time; none is vendored into this repository.

| Package | Used for | Licence |
| --- | --- | --- |
| `Silk.NET.Windowing`, `Silk.NET.OpenGL`, `Silk.NET.Input` | Window, GL context and input in the desktop host | MIT |
| `FontStashSharp` | Glyph atlas packing and text layout | MIT |
| `StbImageWriteSharp` | PNG encoding for screenshots | Public domain / MIT |
| `xunit`, `Microsoft.NET.Test.Sdk` | Tests only; not shipped | Apache-2.0 / MIT |

`BeyondNations.Core` has **no** package dependencies at all. That is deliberate:
the simulation is meant to build and run with nothing but the .NET SDK.
