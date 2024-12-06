using System.Collections.Generic;
using UnityEngine;

public class TIFU_MapData : MonoBehaviour
{
    //map
    [SerializeField] bool isometric = false;
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] int tileWidth;
    [SerializeField] int tileHeight;
    [SerializeField] List<List<int>> tileInstanceLayers = new List<List<int>>();
    [SerializeField] List<List<Vector3Int>> tileInstanceFlips = new List<List<Vector3Int>>();

    //tiles
    [SerializeField] List<int> tileDataIndex = new List<int>();
    [SerializeField] List<Sprite> sprites = new List<Sprite>();

    //tile data
    [SerializeField] List<List<Vector4>> boxColliders = new List<List<Vector4>>();
    [SerializeField] List<List<Vector4>> ellipseColliders = new List<List<Vector4>>();
    [SerializeField] List<List<List<Vector2>>> polygonColliders = new List<List<List<Vector2>>>();
    [SerializeField] List<int> tileProperties = new List<int>();
    [SerializeField] List<int[]> wangTileProperties = new List<int[]>();

    //tile properties
    [SerializeField] List<List<string>> boolProperties = new List<List<string>>();
    [SerializeField] List<List<string>> intProperties = new List<List<string>>();
    [SerializeField] List<List<string>> floatProperties = new List<List<string>>();
    [SerializeField] List<List<string>> colorProperties = new List<List<string>>();
    [SerializeField] List<List<string>> stringProperties = new List<List<string>>();
    //tile property values
    [SerializeField] List<List<bool>> boolPropertyValues = new List<List<bool>>();
    [SerializeField] List<List<int>> intPropertyValues = new List<List<int>>();
    [SerializeField] List<List<float>> floatPropertyValues = new List<List<float>>();
    [SerializeField] List<List<Color>> colorPropertyValues = new List<List<Color>>();
    [SerializeField] List<List<string>> stringPropertyValues = new List<List<string>>();

    //wang properties
    [SerializeField] List<List<string>> boolPropertiesWang = new List<List<string>>();
    [SerializeField] List<List<string>> intPropertiesWang = new List<List<string>>();
    [SerializeField] List<List<string>> floatPropertiesWang = new List<List<string>>();
    [SerializeField] List<List<string>> colorPropertiesWang = new List<List<string>>();
    [SerializeField] List<List<string>> stringPropertiesWang = new List<List<string>>();
    //wang property values
    [SerializeField] List<List<bool>> boolPropertyValuesWang = new List<List<bool>>();
    [SerializeField] List<List<int>> intPropertyValuesWang = new List<List<int>>();
    [SerializeField] List<List<float>> floatPropertyValuesWang = new List<List<float>>();
    [SerializeField] List<List<Color>> colorPropertyValuesWang = new List<List<Color>>();
    [SerializeField] List<List<string>> stringPropertyValuesWang = new List<List<string>>();

    public int GetMapWidth()
    {
        return width;
    }
    public int GetMapHeight()
    {
        return height;
    }
    public int GetTileWidth()
    {
        return tileWidth;
    }
    public int GetTileHeight()
    {
        return tileHeight;
    }

    public int GetTileInstance(Vector3 position, int layer)
    {
        int index = GetTileIndexFromPosition(position);
        return tileInstanceLayers[layer][index];
    }

    public Vector3Int GetTileInstanceFlip(Vector3 position, int layer)
    {
        int index = GetTileIndexFromPosition(position);
        return tileInstanceFlips[layer][index];
    }

    public List<List<int>> GetTileLayers()
    {
        return tileInstanceLayers;
    }
    public List<List<Vector3Int>> GetTileFlips()
    {
        return tileInstanceFlips;
    }

    public void AddTileInstance(int tile, int layer, Vector3Int tileflip)
    {
        while (tileInstanceLayers.Count <= layer)
        {
            tileInstanceLayers.Add(new List<int>());
            tileInstanceFlips.Add(new List<Vector3Int>());
        }
        tileInstanceLayers[layer].Add(tile);
        tileInstanceFlips[layer].Add(tileflip);
    }

    public void SetMapInfo(int width, int height, int tileWidth, int tileHeight, bool isometric)
    {
        this.width = width;
        this.height = height;
        this.tileWidth = tileWidth;
        this.tileHeight = tileHeight;
        this.isometric = isometric;
    }

    public Vector3 GetTileInstancePosition(int instanceindex)
    {
        Vector3 position = new Vector3((instanceindex % width), height - 1 - (instanceindex / width));
        if (isometric)
        {
            position.x = (position.x + position.y) * 0.5f;
            position.y = (position.y - position.x) * 0.5f;
        }
        return position;
    }

    public int GetTileIndexFromPosition(Vector3 position)
    {
        if (isometric)
        {
            //to orthographic
            position.x = position.x - 2 * position.y;
            position.y = 4 * position.y + position.x;
        }
        return ((height-1) - ((int)position.y - (int)transform.position.y)) * (width) + ((int)position.x - (int)transform.position.x);
    }

    public int AddTile(Sprite sprite)
    {
        tileDataIndex.Add(-1);
        sprites.Add(sprite);
        return tileDataIndex.Count - 1;
    }

    public Sprite GetSprite(int tile)
    {
        return sprites[tile];
    }

    int AddGetDataEntry(int tile)
    {
        //add new data entry
        if (tileDataIndex[tile] == -1)
        {
            tileDataIndex[tile] = boxColliders.Count;

            boxColliders.Add(new List<Vector4>());
            ellipseColliders.Add(new List<Vector4>());
            polygonColliders.Add(new List<List<Vector2>>());
            tileProperties.Add(-1);
            wangTileProperties.Add(new int[8]);
        }

        return tileDataIndex[tile];
    }

    public void AddWangTile(int tile, int[] wangIDs)
    {
        int dataindex = AddGetDataEntry(tile);
        wangTileProperties[dataindex] = wangIDs;
    }

    void AddPropertyEntryWang(int wangID)
    {
        while (boolPropertiesWang.Count <= wangID)
        {
            boolPropertiesWang.Add(new List<string>());
            intPropertiesWang.Add(new List<string>());
            floatPropertiesWang.Add(new List<string>());
            colorPropertiesWang.Add(new List<string>());
            stringPropertiesWang.Add(new List<string>());

            boolPropertyValuesWang.Add(new List<bool>());
            intPropertyValuesWang.Add(new List<int>());
            floatPropertyValuesWang.Add(new List<float>());
            colorPropertyValuesWang.Add(new List<Color>());
            stringPropertyValuesWang.Add(new List<string>());
        }
    }

    int AddGetPropertyEntry(int dataIndex)
    {
        if (tileProperties[dataIndex] == -1)
        {
            tileProperties[dataIndex] = boolProperties.Count;

            boolProperties.Add(new List<string>());
            intProperties.Add(new List<string>());
            floatProperties.Add(new List<string>());
            colorProperties.Add(new List<string>());
            stringProperties.Add(new List<string>());

            boolPropertyValues.Add(new List<bool>());
            intPropertyValues.Add(new List<int>());
            floatPropertyValues.Add(new List<float>());
            colorPropertyValues.Add(new List<Color>());
            stringPropertyValues.Add(new List<string>());
        }
        return tileProperties[dataIndex];
    }

    public bool HasWangData(int tile)
    {
        int dataindex = tileDataIndex[tile];
        if (0 <= dataindex)
        {
            foreach (int wang in wangTileProperties[dataindex])
            {
                if (0 < wang)
                {
                    if (0 < boolPropertiesWang[wang].Count || 0 < intPropertiesWang[wang].Count || 0 < floatPropertiesWang[wang].Count || 0 < colorPropertiesWang[wang].Count || 0 < stringPropertiesWang[wang].Count)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void AddBoxColliderData(int tile, Vector4 colliderBounds)
    {
        int dataindex = AddGetDataEntry(tile);
        boxColliders[dataindex].Add(colliderBounds);
    }
    public List<Vector4> GetBoxColliderData(int tile)
    {
        List<Vector4> colliders = new List<Vector4>();
        int dataindex = tileDataIndex[tile];
        if (0 <= dataindex)
        {
            colliders = boxColliders[dataindex];
        }
        return colliders;
    }

    public void AddEllipseColliderData(int tile, Vector4 colliderBounds)
    {
        int dataindex = AddGetDataEntry(tile);
        ellipseColliders[dataindex].Add(colliderBounds);
    }
    public List<Vector4> GetEllipseColliderData(int tile)
    {
        List<Vector4> colliders = new List<Vector4>();
        int dataindex = tileDataIndex[tile];
        if (0 <= dataindex)
        {
            colliders = ellipseColliders[dataindex];
        }
        return colliders;
    }

    public void AddPolygonColliderData(int tile, List<Vector2> colliderVertices)
    {
        int dataindex = AddGetDataEntry(tile);
        polygonColliders[dataindex].Add(colliderVertices);
    }

    public List<List<Vector2>> GetPolygonColliderData(int tile)
    {
        List<List<Vector2>> colliders = new List<List<Vector2>>();
        int dataindex = tileDataIndex[tile];
        if (0 <= dataindex)
        {
            colliders = polygonColliders[dataindex];
        }
        return colliders;
    }

    public void AddBoolProperty(int tile, string property, bool value)
    {
        int dataindex = AddGetDataEntry(tile);
        int propertyIndex = AddGetPropertyEntry(dataindex);
        boolProperties[propertyIndex].Add(property);
        boolPropertyValues[propertyIndex].Add(value);
    }

    public void AddIntProperty(int tile, string property, int value)
    {
        int dataindex = AddGetDataEntry(tile);
        int propertyIndex = AddGetPropertyEntry(dataindex);
        intProperties[propertyIndex].Add(property);
        intPropertyValues[propertyIndex].Add(value);
    }

    public void AddFloatProperty(int tile, string property, float value)
    {
        int dataindex = AddGetDataEntry(tile);
        int propertyIndex = AddGetPropertyEntry(dataindex);
        floatProperties[propertyIndex].Add(property);
        floatPropertyValues[propertyIndex].Add(value);
    }

    public void AddColorProperty(int tile, string property, Color value)
    {
        int dataindex = AddGetDataEntry(tile);
        int propertyIndex = AddGetPropertyEntry(dataindex);
        colorProperties[propertyIndex].Add(property);
        colorPropertyValues[propertyIndex].Add(value);
    }

    public void AddStringProperty(int tile, string property, string value)
    {
        int dataindex = AddGetDataEntry(tile);
        int propertyIndex = AddGetPropertyEntry(dataindex);
        stringProperties[propertyIndex].Add(property);
        stringPropertyValues[propertyIndex].Add(value);
    }

    int GetPropertyIndex(int tile)
    {
        int dataindex = tileDataIndex[tile];
        if (dataindex != -1)
        {
            return tileProperties[dataindex];
        }
        return -1;
    }

    int GetPropertyEntryIndex(int tile, string property, List<List<string>> entryList)
    {
        int propertyIndex = GetPropertyIndex(tile);
        if (propertyIndex != -1)
        {
            return entryList[propertyIndex].FindIndex((string str) => (str == property));
        }
        return -1;
    }

    public bool TryGetBoolProperty(int tile, string property, out bool value)
    {
        value = false;
        int propertyIndex = GetPropertyEntryIndex(tile, property, boolProperties);
        if (-1 < propertyIndex)
        {
            value = boolPropertyValues[tileProperties[tileDataIndex[tile]]][propertyIndex];
            return true;
        }
        return false;
    }

    public bool TryGetIntProperty(int tile, string property, out int value)
    {
        value = -1;
        int propertyIndex = GetPropertyEntryIndex(tile, property, intProperties);
        if (-1 < propertyIndex)
        {
            value = intPropertyValues[tileProperties[tileDataIndex[tile]]][propertyIndex];
            return true;
        }
        return false;
    }

    public bool TryGetFloatProperty(int tile, string property, out float value)
    {
        value = -1;
        int propertyIndex = GetPropertyEntryIndex(tile, property, floatProperties);
        if (-1 < propertyIndex)
        {
            value = floatPropertyValues[tileProperties[tileDataIndex[tile]]][propertyIndex];
            return true;
        }
        return false;
    }

    public bool TryGetColorProperty(int tile, string property, out Color value)
    {
        value = Color.black;
        int propertyIndex = GetPropertyEntryIndex(tile, property, colorProperties);
        if (-1 < propertyIndex)
        {
            value = colorPropertyValues[tileProperties[tileDataIndex[tile]]][propertyIndex];
            return true;
        }
        return false;
    }

    public bool TryGetStringProperty(int tile, string property, out string value)
    {
        value = "";
        int propertyIndex = GetPropertyEntryIndex(tile, property, stringProperties);
        if (-1 < propertyIndex)
        {
            value = stringPropertyValues[tileProperties[tileDataIndex[tile]]][propertyIndex];
            return true;
        }
        return false;
    }


    public int AddBoolPropertyWang(int wangID, string property, bool value)
    {
        AddPropertyEntryWang(wangID);
        boolPropertiesWang[wangID].Add(property);
        boolPropertyValuesWang[wangID].Add(value);
        return boolPropertyValuesWang[wangID].Count - 1;
    }

    public int AddIntPropertyWang(int wangID, string property, int value)
    {
        AddPropertyEntryWang(wangID);
        intPropertiesWang[wangID].Add(property);
        intPropertyValuesWang[wangID].Add(value);
        return intPropertyValuesWang[wangID].Count - 1;
    }

    public int AddFloatPropertyWang(int wangID, string property, float value)
    {
        AddPropertyEntryWang(wangID);
        floatPropertiesWang[wangID].Add(property);
        floatPropertyValuesWang[wangID].Add(value);
        return floatPropertyValuesWang[wangID].Count - 1;
    }

    public int AddColorPropertyWang(int wangID, string property, Color value)
    {
        AddPropertyEntryWang(wangID);
        colorPropertiesWang[wangID].Add(property);
        colorPropertyValuesWang[wangID].Add(value);
        return colorPropertyValuesWang[wangID].Count - 1;
    }

    public int AddStringPropertyWang(int wangID, string property, string value)
    {
        AddPropertyEntryWang(wangID);
        stringPropertiesWang[wangID].Add(property);
        stringPropertyValuesWang[wangID].Add(value);
        return stringPropertyValuesWang[wangID].Count - 1;
    }

    public int[] GetWangIDs(int tile)
    {
        int dataindex = tileDataIndex[tile];
        if (dataindex == -1)
        {
            return new int[0];
        }
        return wangTileProperties[dataindex];
    }

    int GetPropertyIndexWang(int tile, int wangIndex, string property, List<List<string>> entryList)
    {
        int[] wangIds = GetWangIDs(tile);
        if (wangIndex < wangIds.Length)
        {
            int wangId = wangIds[wangIndex];
            if (0 < wangId)
            {
                return entryList[wangId].FindIndex((string str) => (str == property));
            }
        }
        return -1;
    }

    public bool TryGetBoolProperty(int tile, int wangIndex, string property, out bool value)
    {
        value = false;
        int propertyIndex = GetPropertyIndexWang(tile, wangIndex, property, boolPropertiesWang);
        if (-1 < propertyIndex)
        {
            value = boolPropertyValuesWang[GetWangIDs(tile)[wangIndex]][propertyIndex];
            return true;
        }
        return false;
    }

    public bool TryGetIntProperty(int tile, int wangIndex, string property, out int value)
    {
        value = -1;
        int propertyIndex = GetPropertyIndexWang(tile, wangIndex, property, intPropertiesWang);
        if (-1 < propertyIndex)
        {
            value = intPropertyValuesWang[GetWangIDs(tile)[wangIndex]][propertyIndex];
            return true;
        }
        return false;
    }

    public bool TryGetFloatProperty(int tile, int wangIndex, string property, out float value)
    {
        value = -1;
        int propertyIndex = GetPropertyIndexWang(tile, wangIndex, property, floatPropertiesWang);
        if (-1 < propertyIndex)
        {
            value = floatPropertyValuesWang[GetWangIDs(tile)[wangIndex]][propertyIndex];
            return true;
        }
        return false;
    }

    public bool TryGetColorProperty(int tile, int wangIndex, string property, out Color value)
    {
        value = Color.black;
        int propertyIndex = GetPropertyIndexWang(tile, wangIndex, property, colorPropertiesWang);
        if (-1 < propertyIndex)
        {
            value = colorPropertyValuesWang[GetWangIDs(tile)[wangIndex]][propertyIndex];
            return true;
        }
        return false;
    }

    public bool TryGetStringProperty(int tile, int wangIndex, string property, out string value)
    {
        value = "";
        int propertyIndex = GetPropertyIndexWang(tile, wangIndex, property, stringPropertiesWang);
        if (-1 < propertyIndex)
        {
            value = stringPropertyValuesWang[GetWangIDs(tile)[wangIndex]][propertyIndex];
            return true;
        }
        return false;
    }

}
