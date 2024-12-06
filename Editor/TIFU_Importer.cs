using System;
using UnityEditor;
using UnityEngine;
using System.Xml;
using System.Collections.Generic;
using UnityEditor.AssetImporters;

// Automatically set default texturetype to 'sprite' on import
class MyTexturePostprocessor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        TextureImporter textureImporter = (TextureImporter)assetImporter;
        if (textureImporter.importSettingsMissing)
        {
            textureImporter.textureType = TextureImporterType.Sprite;
        }
    }
}

[ScriptedImporter(1, "tmx")]
public class TIFU_Importer : ScriptedImporter
{
    [SerializeField] UnityEngine.Object tsxDir;
    [SerializeField] UnityEngine.Object imageDir;

    [SerializeField] bool createTiles = true;
    [ConditionalProperty("createTiles")]
    [SerializeField] bool includeSprites = true;
    [ConditionalProperty("createTiles")]
    [SerializeField] bool includeColliders = true;

    public override void OnImportAsset(AssetImportContext ctx)
    {
        //create and set object to asset
        GameObject mapObject = new GameObject();
        TIFU_MapData mapData = mapObject.AddComponent<TIFU_MapData>();
        ctx.AddObjectToAsset("main obj", mapObject);
        ctx.SetMainObject(mapObject);

        //load tmx
        XmlDocument tmx = new XmlDocument();
        tmx.Load(ctx.assetPath);

        //sort out filepaths
        string imageFolderPath = imageDir == null ? System.IO.Path.GetDirectoryName(ctx.assetPath) : AssetDatabase.GetAssetPath(imageDir);
        string tilesetFolderPath = tsxDir == null ? imageFolderPath : AssetDatabase.GetAssetPath(tsxDir);

        //get map values or default to zero
        int mapwidth = tmx["map"].Attributes.GetNamedItem("width") == null ? 0 : int.Parse(tmx["map"].Attributes.GetNamedItem("width").Value);
        int mapheight = tmx["map"].Attributes.GetNamedItem("height") == null ? 0 : int.Parse(tmx["map"].Attributes.GetNamedItem("height").Value);
        int maptilewidth = tmx["map"].Attributes.GetNamedItem("tilewidth") == null ? 0 : int.Parse(tmx["map"].Attributes.GetNamedItem("tilewidth").Value);
        int maptileheight = tmx["map"].Attributes.GetNamedItem("tileheight") == null ? 0 : int.Parse(tmx["map"].Attributes.GetNamedItem("tileheight").Value);
        string orientation = tmx["map"].Attributes.GetNamedItem("orientation") == null ? "orthogonal" : tmx["map"].Attributes.GetNamedItem("orientation").Value;

        //set map info
        mapData.SetMapInfo(mapwidth, mapheight, maptilewidth, maptileheight, orientation == "isometric");

        //get tile layers
        XmlNodeList layerNodess = tmx.GetElementsByTagName("layer");
        for (int layer = 0; layer < layerNodess.Count; layer++)
        {
            string dataStringCSV = layerNodess[layer].SelectSingleNode("data").InnerText;
            UInt32[] layerTiles = System.Array.ConvertAll(dataStringCSV.Split(','), UInt32.Parse);
            for (int i = 0; i < layerTiles.Length; i++)
            {
                //get flip bits
                string flips = Convert.ToString(layerTiles[i], toBase: 2).PadLeft(32, '0');
                int flipX = Convert.ToInt32(flips[0].ToString());
                int flipY = Convert.ToInt32(flips[1].ToString());
                int flipZ = Convert.ToInt32(flips[2].ToString());
                //clear flip bits
                uint mask = 0b_0000_1111_1111_1111_1111_1111_1111_1111;
                layerTiles[i] = layerTiles[i] & mask;

                int tile = (int)layerTiles[i] - 1;
                mapData.AddTileInstance(tile, layer, new Vector3Int(flipY, flipX, flipZ));
            }
        }

        //get tileset nodes
        XmlNodeList tilesetNodes = tmx.GetElementsByTagName("tileset");
        for (int ts = 0; ts < tilesetNodes.Count; ts++)
        {
            XmlNode tilesetNode = tilesetNodes[ts];
            int tilesetFirstGID = int.Parse(tilesetNode.Attributes.GetNamedItem("firstgid").Value);

            //get external tileset source
            XmlNode tilesetSource = tilesetNode.Attributes.GetNamedItem("source");
            XmlDocument tilesetSourceFile = new XmlDocument();
            if (tilesetSource != null)
            {
                //load tsx
                tilesetSourceFile.Load(imageFolderPath + "/" + tilesetSource.InnerXml);
                tilesetNode = tilesetSourceFile.GetElementsByTagName("tileset")[0];
            }

            //get tileset values or default to zero
            string name = tilesetNode.Attributes.GetNamedItem("name") == null ? "" : tilesetNode.Attributes.GetNamedItem("name").Value;
            int tilewidth = tilesetNode.Attributes.GetNamedItem("tilewidth") == null ? 0 : int.Parse(tilesetNode.Attributes.GetNamedItem("tilewidth").Value);
            int tileheight = tilesetNode.Attributes.GetNamedItem("tileheight") == null ? 0 : int.Parse(tilesetNode.Attributes.GetNamedItem("tileheight").Value);
            int spacing = tilesetNode.Attributes.GetNamedItem("spacing") == null ? 0 : int.Parse(tilesetNode.Attributes.GetNamedItem("spacing").Value);
            int margin = tilesetNode.Attributes.GetNamedItem("margin") == null ? 0 : int.Parse(tilesetNode.Attributes.GetNamedItem("margin").Value);
            int tilecount = tilesetNode.Attributes.GetNamedItem("tilecount") == null ? 0 : int.Parse(tilesetNode.Attributes.GetNamedItem("tilecount").Value);
            int columns = tilesetNode.Attributes.GetNamedItem("columns") == null ? 0 : int.Parse(tilesetNode.Attributes.GetNamedItem("columns").Value);

            //get tileset image values
            string imagePath = tilesetNode["image"].Attributes.GetNamedItem("source").InnerText;
            int imageWidth = int.Parse(tilesetNode["image"].Attributes.GetNamedItem("width").InnerText);
            int imageHeight = int.Parse(tilesetNode["image"].Attributes.GetNamedItem("height").InnerText);

            //load texture
            Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(imageFolderPath + "/" + System.IO.Path.GetFileName(imagePath), typeof(Texture2D));

            //create sprites
            int spriteID = 0;
            for (int y = imageHeight - tileheight - margin; margin <= y; y -= (tileheight + spacing))
            {
                for (int x = margin; x < imageWidth; x += (tilewidth + spacing))
                {
                    Sprite sprite = Sprite.Create(texture, new Rect(x, y, tilewidth, tileheight), new Vector2(0.5f,0.5f), 32);
                    sprite.name = name + spriteID;
                    mapData.AddTile(sprite);
                    ctx.AddObjectToAsset(name + spriteID++, sprite);
                }
            }

            //get collision data
            XmlNodeList tileNodes = tilesetNode.SelectNodes("tile");
            for (int i = 0; i < tileNodes.Count; i++)
            {
                int tile = int.Parse(tileNodes[i].Attributes.GetNamedItem("id").InnerText);

                XmlNodeList objects = tileNodes[i]["objectgroup"].GetElementsByTagName("object");
                for (int obj = 0; obj < objects.Count; obj++)
                {
                    int id = objects[obj].Attributes.GetNamedItem("id") == null ? 0 : int.Parse(objects[obj].Attributes.GetNamedItem("id").Value);
                    float x = objects[obj].Attributes.GetNamedItem("x") == null ? 0 : float.Parse(objects[obj].Attributes.GetNamedItem("x").Value);
                    float y = objects[obj].Attributes.GetNamedItem("y") == null ? 0 : float.Parse(objects[obj].Attributes.GetNamedItem("y").Value);
                    float width = objects[obj].Attributes.GetNamedItem("width") == null ? 0 : float.Parse(objects[obj].Attributes.GetNamedItem("width").Value);
                    float height = objects[obj].Attributes.GetNamedItem("height") == null ? 0 : float.Parse(objects[obj].Attributes.GetNamedItem("height").Value);

                    //rect/ellipse colliders
                    if (0 < width)
                    {
                        if (objects[obj]["ellipse"] == null)
                        {
                            mapData.AddBoxColliderData(tile + tilesetFirstGID - 1, new Vector4(x - (tilewidth - width) * 0.5f, tileheight*0.5f - (height / 2.0f) - y, width, height) / maptilewidth);
                        }
                        else
                        {
                            mapData.AddEllipseColliderData(tile + tilesetFirstGID - 1, new Vector4(x - (tilewidth - width) * 0.5f, tileheight * 0.5f - (height / 2.0f) - y, width, height) / maptilewidth);
                        }
                    }

                    //polygon colliders
                    XmlNodeList polygons = objects[obj].SelectNodes("polygon");
                    for (int polygon = 0; polygon < polygons.Count; polygon++)
                    {
                        string[] pointStrings = polygons[polygon].Attributes.GetNamedItem("points").InnerText.Split(' ');
                        if (2 < pointStrings.Length)
                        {
                            List<Vector2> vertices = new List<Vector2>();
                            for (int point = 0; point < pointStrings.Length; point++)
                            {
                                float[] vals = System.Array.ConvertAll(pointStrings[point].Split(','), float.Parse);
                                vertices.Add(new Vector2(x + vals[0] - tilewidth * 0.5f, maptilewidth * 0.5f - (y + vals[1])) / maptilewidth);
                            }
                            mapData.AddPolygonColliderData(tile + tilesetFirstGID - 1, vertices);
                        }
                    }
                    XmlNodeList polylines = objects[obj].SelectNodes("polyline");
                    for (int polyline = 0; polyline < polylines.Count; polyline++)
                    {
                        string[] pointStrings = polylines[polyline].Attributes.GetNamedItem("points").InnerText.Split(' ');
                        if (2 < pointStrings.Length)
                        {
                            List<Vector2> vertices = new List<Vector2>();
                            for (int point = 0; point < pointStrings.Length; point++)
                            {
                                float[] vals = System.Array.ConvertAll(pointStrings[point].Split(','), float.Parse);
                                vertices.Add(new Vector2(x + vals[0] - tilewidth * 0.5f, maptilewidth * 0.5f - (y + vals[1])) / maptilewidth);
                            }
                            mapData.AddPolygonColliderData(tile + tilesetFirstGID - 1, vertices);
                        }
                    }

                    //get custom property data
                    if (objects[obj]["properties"] != null)
                    {
                        XmlNodeList properties = objects[obj]["properties"].GetElementsByTagName("property");
                        for (int property = 0; property < properties.Count; property++)
                        {
                            string propertyName = properties[property].Attributes.GetNamedItem("name") == null ? "" : properties[property].Attributes.GetNamedItem("name").Value;
                            string propertyType = properties[property].Attributes.GetNamedItem("type") == null ? "string" : properties[property].Attributes.GetNamedItem("type").Value;
                            string propertyValue = properties[property].Attributes.GetNamedItem("value") == null ? "" : properties[property].Attributes.GetNamedItem("value").Value;

                            switch (propertyType)
                            {
                                case "bool":
                                    mapData.AddBoolProperty(tile + tilesetFirstGID - 1, propertyName, bool.Parse(propertyValue));
                                    break;
                                case "int":
                                    mapData.AddIntProperty(tile + tilesetFirstGID - 1, propertyName, int.Parse(propertyValue));
                                    break;
                                case "float":
                                    mapData.AddFloatProperty(tile + tilesetFirstGID - 1, propertyName, float.Parse(propertyValue));
                                    break;
                                case "color":
                                    {
                                        Color32 color = new Color32(System.Convert.ToByte(propertyValue.Substring(1, 2), 16), System.Convert.ToByte(propertyValue.Substring(3, 2), 16), System.Convert.ToByte(propertyValue.Substring(5, 2), 16), System.Convert.ToByte(propertyValue.Substring(7, 2), 16));
                                        mapData.AddColorProperty(tile + tilesetFirstGID - 1, propertyName, color);
                                    }
                                    break;
                                case "string":
                                    mapData.AddStringProperty(tile + tilesetFirstGID - 1, propertyName, propertyValue);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }

            //get wang custom property data
            XmlNodeList wangsetNodes = tilesetSourceFile.GetElementsByTagName("wangset");
            for (int wangset = 0; wangset < wangsetNodes.Count; wangset++)
            {
                XmlNodeList wangcolors = wangsetNodes[wangset].SelectNodes("wangcolor");
                for (int wangcolor = 0; wangcolor < wangcolors.Count; wangcolor++)
                {
                    XmlNodeList properties = wangcolors[wangcolor].SelectNodes("properties/property");
                    for (int property = 0; property < properties.Count; property++)
                    {
                        string propertyName = properties[property].Attributes.GetNamedItem("name") == null ? "" : properties[property].Attributes.GetNamedItem("name").Value;
                        string propertyType = properties[property].Attributes.GetNamedItem("type") == null ? "string" : properties[property].Attributes.GetNamedItem("type").Value;
                        string propertyValue = properties[property].Attributes.GetNamedItem("value") == null ? "" : properties[property].Attributes.GetNamedItem("value").Value;

                        switch (propertyType)
                        {
                            case "bool":
                                mapData.AddBoolPropertyWang(wangcolor + tilesetFirstGID, propertyName, bool.Parse(propertyValue));
                                break;
                            case "int":
                                mapData.AddIntPropertyWang(wangcolor + tilesetFirstGID, propertyName, int.Parse(propertyValue));
                                break;
                            case "float":
                                mapData.AddFloatPropertyWang(wangcolor + tilesetFirstGID, propertyName, float.Parse(propertyValue));
                                break;
                            case "color":
                                Color32 color = new Color32(System.Convert.ToByte(propertyValue.Substring(1, 2), 16), System.Convert.ToByte(propertyValue.Substring(3, 2), 16), System.Convert.ToByte(propertyValue.Substring(5, 2), 16), System.Convert.ToByte(propertyValue.Substring(7, 2), 16));
                                mapData.AddColorPropertyWang(wangcolor + tilesetFirstGID, propertyName, color);
                                break;
                            case "string":
                                mapData.AddStringPropertyWang(wangcolor + tilesetFirstGID, propertyName, propertyValue);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            //get wang tiles
            XmlNodeList wangtileNodes = tilesetSourceFile.GetElementsByTagName("wangtile");
            for (int wangtile = 0; wangtile < wangtileNodes.Count; wangtile++)
            {
                int wangtile_tileID = int.Parse(wangtileNodes[wangtile].Attributes.GetNamedItem("tileid").Value);
                string wangIDstring = wangtileNodes[wangtile].Attributes.GetNamedItem("wangid").Value;
                mapData.AddWangTile(wangtile_tileID, System.Array.ConvertAll(wangIDstring.Split(','), int.Parse));
            }
        }

        //Create tile gameobjects
        if (createTiles)
        {
            int tileObjectID = 0;
            List<List<int>> layers = mapData.GetTileLayers();
            List<List<Vector3Int>> flips = mapData.GetTileFlips();
            for (int layer = 0; layer < layers.Count; layer++)
            {
                for (int i = 0; i < layers[layer].Count; i++)
                {
                    int tile = layers[layer][i];
                    if (0 <= tile)
                    {
                        GameObject tileObject = new GameObject("tile_" + tileObjectID++);
                        //set transform
                        tileObject.transform.SetParent(mapObject.transform);
                        tileObject.transform.position = mapData.GetTileInstancePosition(i);
                        tileObject.transform.Rotate(flips[layer][i] * 180);

                        //add sprite
                        if (includeSprites)
                        {
                            SpriteRenderer spriteRenderer = tileObject.AddComponent<SpriteRenderer>();
                            spriteRenderer.sprite = mapData.GetSprite(tile);
                            spriteRenderer.sortingOrder = layer;
                        }

                        //add colliders
                        if (includeColliders)
                        {
                            foreach (var vertexlist in mapData.GetPolygonColliderData(tile))
                            {
                                tileObject.AddComponent<PolygonCollider2D>().points = vertexlist.ToArray();
                            }
                            foreach (var boxboundary in mapData.GetBoxColliderData(tile))
                            {
                                BoxCollider2D collider = tileObject.AddComponent<BoxCollider2D>();
                                collider.offset = new Vector2(boxboundary.x, boxboundary.y);
                                collider.size = new Vector2(boxboundary.z, boxboundary.w);
                            }
                            foreach (var ellipseboundary in mapData.GetEllipseColliderData(tile))
                            {
                                CapsuleCollider2D collider = tileObject.AddComponent<CapsuleCollider2D>();
                                collider.offset = new Vector2(ellipseboundary.x, ellipseboundary.y);
                                collider.size = new Vector2(ellipseboundary.z, ellipseboundary.w);
                            }
                        }
                    }
                }
            }
        }
    }
}
