/*
Squared.Tiled
Copyright (C) 2009 Kevin Gadd

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.

  Kevin Gadd kevin.gadd@gmail.com http://luminance.org/
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace GameEngineStage6.Utils
{
    public class Tileset
    {
        public class TilePropertyList : Dictionary<string, string>
        {
        }

        public string Name;
        public int FirstTileID;
        public int TileWidth;
        public int TileHeight;
        public int Spacing;
        public int Margin;
        public Dictionary<int, TilePropertyList> TileProperties = new Dictionary<int, TilePropertyList>();
        public string Image;
        public int ImageWidth;
        public int ImageHeight;
        //protected Image _Texture;
        //protected int _TexWidth, _TexHeight;

        internal static Tileset Load(XmlReader reader)
        {
            var result = new Tileset()
            {
                Name = reader.GetAttribute("name"),
                FirstTileID = int.Parse(reader.GetAttribute("firstgid")),
                TileWidth = int.Parse(reader.GetAttribute("tilewidth")),
                TileHeight = int.Parse(reader.GetAttribute("tileheight"))
            };
            int.TryParse(reader.GetAttribute("margin"), out result.Margin);
            int.TryParse(reader.GetAttribute("spacing"), out result.Spacing);

            int currentTileId = -1;

            while (reader.Read())
            {
                var name = reader.Name;

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (name)
                        {
                            case "image":
                                result.Image = reader.GetAttribute("source");
                                result.ImageWidth = int.Parse(reader.GetAttribute("width"));
                                result.ImageHeight = int.Parse(reader.GetAttribute("height"));
                                break;
                            case "tile":
                                currentTileId = int.Parse(reader.GetAttribute("id"));
                                break;
                            case "property":
                                {
                                    if (!result.TileProperties.TryGetValue(currentTileId, out TilePropertyList props))
                                    {
                                        props = new TilePropertyList();
                                        result.TileProperties[currentTileId] = props;
                                    }

                                    props[reader.GetAttribute("name")] = reader.GetAttribute("value");
                                }
                                break;
                        }

                        break;
                    case XmlNodeType.EndElement:
                        break;
                }
            }

            return result;
        }

        public TilePropertyList GetTileProperties(int index)
        {
            index -= FirstTileID;

            if (index < 0)
                return null;

            TileProperties.TryGetValue(index, out TilePropertyList result);

            return result;
        }

        /*
        public Image Texture
        {
            get
            {
                return _Texture;
            }
            set
            {
                _Texture = value;
                _TexWidth = value.Width;
                _TexHeight = value.Height;
            }
        }
        */
        /*
        internal bool MapTileToRect(int index, ref Rectangle rect)
        {
            index -= FirstTileID;

            if (index < 0)
                return false;

            int rowSize = _TexWidth / (TileWidth + Spacing);
            int row = index / rowSize;
            int numRows = _TexHeight / (TileHeight + Spacing);
            if (row >= numRows)
                return false;

            int col = index % rowSize;

            rect.X = col * TileWidth + col * Spacing + Margin;
            rect.Y = row * TileHeight + row * Spacing + Margin;
            rect.Width = TileWidth;
            rect.Height = TileHeight;
            return true;
        }
        */
    }

    public class Layer
    {
        /*
         * High-order bits in the tile data indicate tile flipping
         */
        private const uint FlippedHorizontallyFlag = 0x80000000;
        private const uint FlippedVerticallyFlag = 0x40000000;
        private const uint FlippedDiagonallyFlag = 0x20000000;

        internal const byte HorizontalFlipDrawFlag = 1;
        internal const byte VerticalFlipDrawFlag = 2;
        internal const byte DiagonallyFlipDrawFlag = 4;

        public SortedList<string, string> Properties = new SortedList<string, string>();
        internal struct TileInfo
        {
            public Image Texture;
            public Rectangle Rectangle;
        }

        public string Name;
        public int Width, Height;
        public float Opacity = 1;
        public int[] Tiles;
        public byte[] FlipAndRotate;
        internal TileInfo[] _TileInfoCache = null;

        internal static Layer Load(XmlReader reader)
        {
            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
            var result = new Layer();

            if (reader.GetAttribute("name") != null)
            {
                result.Name = reader.GetAttribute("name");
            }
            if (reader.GetAttribute("width") != null)
            {
                result.Width = int.Parse(reader.GetAttribute("width"));
            }
            if (reader.GetAttribute("height") != null)
            {
                result.Height = int.Parse(reader.GetAttribute("height"));
            }
            if (reader.GetAttribute("opacity") != null)
            {
                result.Opacity = float.Parse(reader.GetAttribute("opacity"), NumberStyles.Any, ci);
            }
            result.Tiles = new int[result.Width * result.Height];
            result.FlipAndRotate = new byte[result.Width * result.Height];

            while (!reader.EOF)
            {
                var name = reader.Name;

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (name)
                        {
                            case "data":
                                {
                                    if (reader.GetAttribute("encoding") != null)
                                    {
                                        var encoding = reader.GetAttribute("encoding");
                                        var compressor = reader.GetAttribute("compression");
                                        switch (encoding)
                                        {
                                            case "base64":
                                                {
                                                    int dataSize = (result.Width * result.Height * 4) + 1024;
                                                    var buffer = new byte[dataSize];
                                                    reader.ReadElementContentAsBase64(buffer, 0, dataSize);

                                                    Stream stream = new MemoryStream(buffer, false);
                                                    if (compressor == "gzip")
                                                        stream = new GZipStream(stream, CompressionMode.Decompress, false);

                                                    using (stream)
                                                    using (var br = new BinaryReader(stream))
                                                    {
                                                        for (int i = 0; i < result.Tiles.Length; i++)
                                                        {
                                                            uint tileData = br.ReadUInt32();

                                                            // The data contain flip information as well as the tileset index
                                                            byte flipAndRotateFlags = 0;
                                                            if ((tileData & FlippedHorizontallyFlag) != 0)
                                                            {
                                                                flipAndRotateFlags |= HorizontalFlipDrawFlag;
                                                            }
                                                            if ((tileData & FlippedVerticallyFlag) != 0)
                                                            {
                                                                flipAndRotateFlags |= VerticalFlipDrawFlag;
                                                            }
                                                            if ((tileData & FlippedDiagonallyFlag) != 0)
                                                            {
                                                                flipAndRotateFlags |= DiagonallyFlipDrawFlag;
                                                            }
                                                            result.FlipAndRotate[i] = flipAndRotateFlags;

                                                            // Clear the flip bits before storing the tile data
                                                            tileData &= ~(FlippedHorizontallyFlag |
                                                                          FlippedVerticallyFlag |
                                                                          FlippedDiagonallyFlag);
                                                            result.Tiles[i] = (int)tileData;
                                                        }
                                                    }

                                                    continue;
                                                };

                                            default:
                                                throw new Exception("Unrecognized encoding.");
                                        }
                                    }
                                    else
                                    {
                                        using (var st = reader.ReadSubtree())
                                        {
                                            int i = 0;
                                            while (!st.EOF)
                                            {
                                                switch (st.NodeType)
                                                {
                                                    case XmlNodeType.Element:
                                                        if (st.Name == "tile")
                                                        {
                                                            if (i < result.Tiles.Length)
                                                            {
                                                                result.Tiles[i] = int.Parse(st.GetAttribute("gid"));
                                                                i++;
                                                            }
                                                        }

                                                        break;
                                                    case XmlNodeType.EndElement:
                                                        break;
                                                }

                                                st.Read();
                                            }
                                        }
                                    }
                                    Console.WriteLine("It made it!");
                                }
                                break;
                            case "properties":
                                {
                                    using (var st = reader.ReadSubtree())
                                    {
                                        while (!st.EOF)
                                        {
                                            switch (st.NodeType)
                                            {
                                                case XmlNodeType.Element:
                                                    if (st.Name == "property")
                                                    {
                                                        if (st.GetAttribute("name") != null)
                                                        {
                                                            result.Properties.Add(st.GetAttribute("name"), st.GetAttribute("value"));
                                                        }
                                                    }

                                                    break;
                                                case XmlNodeType.EndElement:
                                                    break;
                                            }

                                            st.Read();
                                        }
                                    }
                                }
                                break;
                        }

                        break;
                    case XmlNodeType.EndElement:
                        break;
                }

                reader.Read();
            }

            return result;
        }

        public int GetTile(int x, int y)
        {
            if ((x < 0) || (y < 0) || (x >= Width) || (y >= Height))
                throw new InvalidOperationException();

            int index = (y * Width) + x;
            return Tiles[index];
        }

        /*
        protected void BuildTileInfoCache(IList<Tileset> tilesets)
        {
            Rectangle rect = new Rectangle();
            var cache = new List<TileInfo>();
            int i = 1;

        next:
            for (int t = 0; t < tilesets.Count; t++)
            {
                if (tilesets[t].MapTileToRect(i, ref rect))
                {
                    cache.Add(new TileInfo
                    {
                        Texture = tilesets[t].Texture,
                        Rectangle = rect
                    });
                    i += 1;
                    goto next;
                }
            }

            _TileInfoCache = cache.ToArray();
        }
        */

    }

    public class ObjectGroup
    {
        public SortedList<string, Object> Objects = new SortedList<string, Object>();
        public SortedList<string, string> Properties = new SortedList<string, string>();

        public string Name;
        public int Width, Height, X, Y;
        float Opacity = 1;

        internal static ObjectGroup Load(XmlReader reader)
        {
            var result = new ObjectGroup();
            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";

            if (reader.GetAttribute("name") != null)
                result.Name = reader.GetAttribute("name");
            if (reader.GetAttribute("width") != null)
                result.Width = int.Parse(reader.GetAttribute("width"));
            if (reader.GetAttribute("height") != null)
                result.Height = int.Parse(reader.GetAttribute("height"));
            if (reader.GetAttribute("x") != null)
                result.X = int.Parse(reader.GetAttribute("x"));
            if (reader.GetAttribute("y") != null)
                result.Y = int.Parse(reader.GetAttribute("y"));
            if (reader.GetAttribute("opacity") != null)
                result.Opacity = float.Parse(reader.GetAttribute("opacity"), NumberStyles.Any, ci);

            while (!reader.EOF)
            {
                var name = reader.Name;

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (name)
                        {
                            case "object":
                                {
                                    using (var st = reader.ReadSubtree())
                                    {
                                        st.Read();
                                        var objects = Object.Load(st);
                                        if (!result.Objects.ContainsKey(objects.Name))
                                        {
                                            result.Objects.Add(objects.Name, objects);
                                        }
                                        else
                                        {
                                            //!!!int count = result.Objects.Keys.Count((item) => item.Equals(objects.Name));
                                            int count = result.Objects.Keys.Count;
                                            result.Objects.Add(string.Format("{0}{1}", objects.Name, count), objects);
                                        }
                                    }
                                }
                                break;
                            case "properties":
                                {
                                    using (var st = reader.ReadSubtree())
                                    {
                                        while (!st.EOF)
                                        {
                                            switch (st.NodeType)
                                            {
                                                case XmlNodeType.Element:
                                                    if (st.Name == "property")
                                                    {
                                                        if (st.GetAttribute("name") != null)
                                                        {
                                                            result.Properties.Add(st.GetAttribute("name"), st.GetAttribute("value"));
                                                        }
                                                    }

                                                    break;
                                                case XmlNodeType.EndElement:
                                                    break;
                                            }

                                            st.Read();
                                        }
                                    }
                                }
                                break;
                        }

                        break;
                    case XmlNodeType.EndElement:
                        break;
                }

                reader.Read();
            }

            return result;
        }


    }

    public class Object
    {
        public SortedList<string, string> Properties = new SortedList<string, string>();

        public string Name, Image;
        public int Width, Height, X, Y;
        public string Points;

        protected Image _Texture;
        protected int _TexWidth, _TexHeight;

        public Image Texture
        {
            get
            {
                return _Texture;
            }
            set
            {
                _Texture = value;
                _TexWidth = value.Width;
                _TexHeight = value.Height;
            }
        }

        internal static Object Load(XmlReader reader)
        {
            var result = new Object()
            {
                Name = reader.GetAttribute("name"),
                X = int.Parse(reader.GetAttribute("x")),
                Y = int.Parse(reader.GetAttribute("y"))
            };

            /*
             * Height and width are optional on objects
             */
            if (int.TryParse(reader.GetAttribute("width"), out int width))
            {
                result.Width = width;
            }

            if (int.TryParse(reader.GetAttribute("height"), out int height))
            {
                result.Height = height;
            }

            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "properties")
                        {
                            using (var st = reader.ReadSubtree())
                            {
                                while (!st.EOF)
                                {
                                    switch (st.NodeType)
                                    {
                                        case XmlNodeType.Element:
                                            if (st.Name == "property")
                                            {
                                                if (st.GetAttribute("name") != null)
                                                {
                                                    result.Properties.Add(st.GetAttribute("name"), st.GetAttribute("value"));
                                                }
                                            }

                                            break;
                                        case XmlNodeType.EndElement:
                                            break;
                                    }

                                    st.Read();
                                }
                            }
                        }
                        if (reader.Name == "image")
                        {
                            result.Image = reader.GetAttribute("source");
                        }
                        if (reader.Name == "polyline")
                        {
                            result.Points = reader.GetAttribute("points");
                        }

                        break;
                    case XmlNodeType.EndElement:
                        break;
                }

                reader.Read();
            }

            return result;
        }


    }

    public class Map
    {
        public SortedList<string, Tileset> Tilesets = new SortedList<string, Tileset>();
        public SortedList<string, Layer> Layers = new SortedList<string, Layer>();
        public SortedList<string, ObjectGroup> ObjectGroups = new SortedList<string, ObjectGroup>();
        public SortedList<string, string> Properties = new SortedList<string, string>();
        public int Width, Height;
        public int TileWidth, TileHeight;

        public static Map Load(string filename/*, ContentManager content*/)
        {
            var result = new Map();
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                //ProhibitDtd = false,
                DtdProcessing = DtdProcessing.Parse
            };
            using (var stream = File.OpenText(filename))
            using (var reader = XmlReader.Create(stream, settings))
                while (reader.Read())
                {
                    var name = reader.Name;

                    switch (reader.NodeType)
                    {
                        case XmlNodeType.DocumentType:
                            if (name != "map")
                                throw new Exception("Invalid map format");
                            break;
                        case XmlNodeType.Element:
                            switch (name)
                            {
                                case "map":
                                    {
                                        result.Width = int.Parse(reader.GetAttribute("width"));
                                        result.Height = int.Parse(reader.GetAttribute("height"));
                                        result.TileWidth = int.Parse(reader.GetAttribute("tilewidth"));
                                        result.TileHeight = int.Parse(reader.GetAttribute("tileheight"));
                                    }
                                    break;
                                case "tileset":
                                    {
                                        using (var st = reader.ReadSubtree())
                                        {
                                            st.Read();
                                            var tileset = Tileset.Load(st);
                                            result.Tilesets.Add(tileset.Name, tileset);
                                        }
                                    }
                                    break;
                                case "layer":
                                    {
                                        using (var st = reader.ReadSubtree())
                                        {
                                            st.Read();
                                            var layer = Layer.Load(st);
                                            if (null != layer)
                                            {
                                                result.Layers.Add(layer.Name, layer);
                                            }
                                        }
                                    }
                                    break;
                                case "objectgroup":
                                    {
                                        using (var st = reader.ReadSubtree())
                                        {
                                            st.Read();
                                            var objectgroup = ObjectGroup.Load(st);
                                            result.ObjectGroups.Add(objectgroup.Name, objectgroup);
                                        }
                                    }
                                    break;
                                case "properties":
                                    {
                                        using (var st = reader.ReadSubtree())
                                        {
                                            while (!st.EOF)
                                            {
                                                switch (st.NodeType)
                                                {
                                                    case XmlNodeType.Element:
                                                        if (st.Name == "property")
                                                        {
                                                            if (st.GetAttribute("name") != null)
                                                            {
                                                                result.Properties.Add(st.GetAttribute("name"), st.GetAttribute("value"));
                                                            }
                                                        }

                                                        break;
                                                    case XmlNodeType.EndElement:
                                                        break;
                                                }

                                                st.Read();
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            break;
                        case XmlNodeType.Whitespace:
                            break;
                    }
                }

            foreach (var tileset in result.Tilesets.Values)
            {
                /*tileset.Texture = content.Load<Image>(
                    Path.Combine(Path.GetDirectoryName(tileset.Image), Path.GetFileNameWithoutExtension(tileset.Image))
                );*/
            }

            foreach (var objects in result.ObjectGroups.Values)
            {
                foreach (var item in objects.Objects.Values)
                {
                    if (item.Image != null)
                    {
                        /*
                        item.Texture = content.Load<Image>
                        (
                            Path.Combine
                            (
                                Path.GetDirectoryName(item.Image),
                                Path.GetFileNameWithoutExtension(item.Image)
                            )
                        );
                        */
                    }
                }
            }

            return result;
        }


    }

}
