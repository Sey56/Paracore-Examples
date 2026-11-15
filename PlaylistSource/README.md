# PlaylistSource

Finished scripts for the three [Playlist tutorials](https://sey56.github.io/paracore-help/docs/tutorials/playlists) on the Paracore help site. This folder is a **script source**: load it in the desktop app, then build playlists out of the scripts that appear in the Gallery.

## Load it

1. Clone or download this repository.
2. In the desktop app: **hamburger menu → Sidebar → Local Sources → Load or Initialize Source**.
3. Select the **`PlaylistSource`** folder — not the repository root.
4. Open **Gallery** to see all five scripts. Switch the TopBar to **Playlists** and start with [Your First Playlist](https://sey56.github.io/paracore-help/docs/tutorials/playlist-01-first-playlist).

A playlist can only contain scripts from the **active script source**, so keep `PlaylistSource` selected for the whole lesson. Playlists you create are saved into this folder as `*.playlist.json` files — they are local to your machine and are not part of this repository.

## What's here

| Script | Reads / writes | Used in |
|---|---|---|
| `WallCountByLevel` | read-only — wall count by level | P1, P3 |
| `RoomAreaReport` | read-only — room area by level | P1 |
| `DoorSchedule` | read-only — doors ordered by width | P1, P3 |
| `StampWallsOnLevel` | **writes** — stamps `Comments` on every wall of one chosen level | P2, P3 |
| `FailCheck` | read-only — throws on purpose to demonstrate stop-on-error | P3 |

`StampWallsOnLevel` has a `Params` form with two inputs: a **Target Level** dropdown and a **Stamp** text box. In a playlist you can add this one script several times and give each step its own level — that is the point of [Lesson P2](https://sey56.github.io/paracore-help/docs/tutorials/playlist-02-one-tool-many-levels).

> ⚠ **P2 and P3 change the model.** Run them on a copy of a project. The stamp write is idempotent (it sets a fixed value), so re-running a playlist does not create anything twice.

See the [Playlists reference](https://sey56.github.io/paracore-help/docs/authoring/playlists) for how playlists are stored and executed.
