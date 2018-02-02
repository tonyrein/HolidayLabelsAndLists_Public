using System.IO;

namespace UI
{
    public static class FilesystemUtils
    {
        public static void MoveWithReplace(string sourcepath, string targetpath)
        {
            if (File.Exists(targetpath))
            {
                File.Delete(targetpath);
            }
            File.Move(sourcepath, targetpath);
        }
    }
}