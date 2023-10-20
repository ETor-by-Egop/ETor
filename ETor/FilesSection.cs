using ETor.BEncoding;

namespace ETor;

public class FilesSection : List<FileSection>
{
    public FilesSection(BEncodeList filesList) : base(filesList.Items.Count)
    {
        if (filesList.Items.Count == 1)
        {
            // single-file mode
            
            if (filesList.Items[0] is not BEncodeDictionary fileDict)
            {
                throw new InvalidOperationException("Files List Item is not a BEncode dictionary");
            }
            
            Add(new SingleFileSection(fileDict));
        }
        else if(filesList.Items.Count > 1)
        {
            foreach (var filesListItem in filesList.Items)
            {
                if (filesListItem is not BEncodeDictionary fileDict)
                {
                    throw new InvalidOperationException("Files List Item is not a BEncode dictionary");
                }

                Add(new MultiFileSection(fileDict));
            }
        }
    }
}