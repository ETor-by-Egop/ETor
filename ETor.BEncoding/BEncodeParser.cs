using System.Text;
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

namespace ETor.BEncoding;

public class BEncodeParser
{
    private readonly byte[] _bytes;

    public BEncodeParser(byte[] bytes)
    {
        _bytes = bytes;
        _position = 0;
    }

    private int _position;

    public void Adjust(int offset)
    {
        _position += offset;
    }

    public BEncodeTokenType GetNextTokenType()
    {
        var c = _bytes[_position];

        return c switch
        {
            (byte) 'd' => BEncodeTokenType.Dictionary,
            (byte) 'e' => BEncodeTokenType.End,
            (byte) 'i' => BEncodeTokenType.Integer,
            (byte) 'l' => BEncodeTokenType.List,
            _ => BEncodeTokenType.Unknown
        };
    }

    public BEncodeDictionary ReadDictionary()
    {
        if (GetNextTokenType() is not BEncodeTokenType.Dictionary)
        {
            throw new InvalidOperationException("not a dictionary");
        }

        Adjust(1);

        var dictionary = new BEncodeDictionary();

        while (GetNextTokenType() is not BEncodeTokenType.End)
        {
            var key = ReadString().Value.ToString();

            var val = ReadNextNode();

            dictionary.Items[key] = val;
        }
        Adjust(1);

        return dictionary;
    }

    public BEncodeNode ReadNextNode()
    {
        var type = GetNextTokenType();

        if (type is BEncodeTokenType.Unknown)
        {
            type = BEncodeTokenType.String;
        }

        var val = ReadToken(type);

        return val;
    }

    private BEncodeNode ReadToken(BEncodeTokenType type)
    {
        switch (type)
        {
            case BEncodeTokenType.String:
            {
                return ReadString();
            }
            case BEncodeTokenType.List:
            {
                return ReadList();
            }
            case BEncodeTokenType.Dictionary:
            {
                return ReadDictionary();
            }
            case BEncodeTokenType.Integer:
            {
                return ReadInteger();
            }
            case BEncodeTokenType.Unknown:
            case BEncodeTokenType.End:
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    private BEncodeNode ReadList()
    {
        Adjust(1);
        var list = new BEncodeList();

        while (GetNextTokenType() is not BEncodeTokenType.End)
        {
            list.Items.Add(ReadNextNode());
        }
        Adjust(1);

        return list;
    }

    public BEncodeString ReadString()
    {
        var colonIndex = FindNextChar(':');

        if (colonIndex == -1)
        {
            throw new InvalidOperationException("colonindex -1");
        }

        var lenChars = colonIndex - _position;

        var len = Encoding.UTF8.GetString(_bytes, _position, lenChars);

        Adjust(lenChars + 1);

        var strLen = int.Parse(len);

        var value = new ByteString(_bytes, _position, strLen);

        Adjust(strLen);

        return new BEncodeString()
        {
            Value = value
        };
    }

    public BEncodeInteger ReadInteger()
    {
        Adjust(1);

        var endIndex = FindNextChar('e');

        if (endIndex <= 0)
        {
            throw new InvalidOperationException("endindex <= 0");
        }

        int len = endIndex - _position;
        
        var content = Encoding.UTF8.GetString(_bytes, _position, len);

        var num = long.Parse(content);

        Adjust(len + 1);

        return new BEncodeInteger(num);
    }

    public int FindNextChar(char c)
    {
        int p = _position;
        while (p < _bytes.Length)
        {
            if (_bytes[p] == c)
            {
                return p;
            }

            p++;
        }

        return -1;
    }
}