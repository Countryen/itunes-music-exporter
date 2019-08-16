# Under Development
Please don't use the tool as it's still not finished.

# Description
Just a little helper tool, created in my freetime for a friend of mine using a Mac and iTunes.
Developed in C# with .NET Core because target platform is Mac OS.

# Explanation
iTunes stores the music in a special folder on the Mac.
It stores information (especially Playlists) in an XML file.
This tool just looks through a given XML file.
It then can export (or rather copy) all the music to a folder of your choice.
It can also export (or rather copy) all the music of a specific playlist (or all at ones) to a folder of your choice.

This basically allows you to have an iTunes playlist, and "export" this playlist. The files are already on your Mac.
Afterwards, you can copy the (playlist-)folders to another device of your choice.

# Usage
Coming soon.

# About: XML file
## Basic Structure
- A bit weird of an XML structure, but easy to read (not that easy to parse, though)
- There are key-value-pairs, that don't sit inside a bucket sorta element, the "key"-element is just always preceding the "value"-element.
- The "value"-element is actually always the TYPE of the value (integer, string, array, true, etc.)
- It consists of meta data at the top, then an "dict" with the Tracks and an "array" with the Playlists. It ends with another meta-item, the "Music Folder".
- !DOCTYPE plist PUBLIC "-//Apple Computer//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd"
```
<dict>
  <key>...</key><value>...</value>
  <key>...</key><value>...</value>
  <key>...</key><value>...</value>
  <key>...</key><value>...</value>
  ...  
  <key>Tracks</key>
    <dict>...</dict>
  <key>Playlists</key>
    <array>...</array>
  <key>Music Folder</key><string>file:///Users/testuser/Music/iTunes/iTunes%20Media/</string>
</dict>
```

## Key: Tracks
- Same rules as above, just different structure.
- First the "key" as an integer value, then the value as a dict.
- the dict is just different meta key-value-pairs as above
- Mostly important: "Name" and "Location"
```
<dict>
  <key>1000</key>
  <dict>
    <key>...</key><value>...</value>
    ...
    <key>Name</key><string>Music Title 1</string>
    <key>Location</key><string>file:///Users/testuser/Music/iTunes/iTunes%20Media/.../Music_Title_1.mp3</string>
  </dict>
  
  <key>...</key>
  <dict>
    ...
  </dict>
  ...
</dict>
```

## Key: Playlists
- Around the same as Tracks, but without a preceding "key> for every item.
- There seems to be a "Master"-item with "All Items" = true and "Name" = "Mediathek". This one is a bit different than the others (meta data)
- Each item contains a "array"-item, which contains the track ids
- Tracks are inside another dict with exactly 1 key-value-pair, "Track ID" beeing the key and an integer is the value (referencing the track from above)
```
<array>
  <dict>
    <key>...</key><value>...</value>
    ...
    <key>Name</key><string>Playlist 1</string>
    <key>Playlist Items</key>
    <array>
      <dict><key>Track ID</key><integer>1000</integer>
      <dict><key>Track ID</key><integer>...</integer>
      ...
    </array>
  </dict>
  
  <dict>
    ...
  </dict>
  ...
</array>
```

## Example (see more in Data-Folder)
```
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple Computer//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>Major Version</key><integer>1</integer>
    <key>Minor Version</key><integer>1</integer>
    <key>Application Version</key><string>12.5.1.21</string>
    <key>Date</key><date>2019-08-14T21:01:18Z</date>
    <key>Features</key><integer>5</integer>
    <key>Show Content Ratings</key><true/>
    <key>Library Persistent ID</key><string>*****</string>
    <key>Tracks</key>
    <dict>
        <key>1000</key>
            <dict>
                <key>Track ID</key><integer>1000</integer>
                <key>Size</key><integer>4348900</integer>
                <key>Total Time</key><integer>180088</integer>
                <key>Date Modified</key><date>2015-03-08T10:18:14Z</date>
                <key>Date Added</key><date>2015-03-08T10:32:22Z</date>
                <key>Bit Rate</key><integer>192</integer>
                <key>Sample Rate</key><integer>44100</integer>
                <key>Play Count</key><integer>13</integer>
                <key>Play Date</key><integer>3550609495</integer>
                <key>Play Date UTC</key><date>2016-07-05T22:24:55Z</date>
                <key>Skip Count</key><integer>3</integer>
                <key>Skip Date</key><date>2015-05-29T18:24:04Z</date>
                <key>Artwork Count</key><integer>1</integer>
                <key>Persistent ID</key><string>*****</string>
                <key>Track Type</key><string>File</string>
                <key>File Folder Count</key><integer>5</integer>
                <key>Library Folder Count</key><integer>1</integer>
                <key>Name</key><string>Title 1</string>
                <key>Artist</key><string>The Best Artist</string>
                <key>Kind</key><string>MPEG-Audiodatei</string>
                <key>Location</key><string>file:///Users/testuser/Music/iTunes/iTunes%20Media/Music/The%20Best%20Artist/Unknown%20Album/Title_1.mp3</string>
            </dict>
        </dict>
    <key>Playlists</key>
    <array>
        <dict>
            <key>Master</key><true/>
            <key>Playlist ID</key><integer>5000</integer>
            <key>Playlist Persistent ID</key><string>*****</string>
            <key>All Items</key><true/>
            <key>Visible</key><false/>
            <key>Name</key><string>Mediathek</string>
            <key>Playlist Items</key>
            <array>
                <dict>
                    <key>Track ID</key><integer>1000</integer>
                </dict>
            </array>
        </dict>
        <dict>
            <key>Playlist ID</key><integer>6000</integer>
            <key>Playlist Persistent ID</key><string>*****</string>
            <key>Distinguished Kind</key><integer>4</integer>
            <key>Music</key><true/>
            <key>All Items</key><true/>
            <key>Name</key><string>Best Artist</string>
            <key>Smart Info</key>
            <data>
            AQEAAwAAAAIAAAAZAAAAAAAAAAcAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            AAAAAA==
            </data>
            <key>Smart Criteria</key>
            <data>
            U0xzdAABAAEAAAACAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADwAAAQAAAAAAAAAAAAAAAAAAAAAAAAA
            AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABEAAAAAAAQIbEAAAAAAAAAAAAAAAAAAAAB
            AAAAAAAQIbEAAAAAAAAAAAAAAAAAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA8AgAEAAAA
            AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAARAAAAAAAIIAE
            AAAAAAAAAAAAAAAAAAAAAQAAAAAAIIAEAAAAAAAAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAA
            AAAAAAAA
            </data>
            <key>Playlist Items</key>
            <array>
                <dict>
                    <key>Track ID</key><integer>1000</integer>
                </dict>
            </array>
        </dict>
    </array>
    <key>Music Folder</key><string>file:///Users/testuser/Music/iTunes/iTunes%20Media/</string>
</dict>
</plist>
```
