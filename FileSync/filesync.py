import sys
import os
import shutil
import tkinter
from tkinter import messagebox


def filesysnc(fromDir, toDir, extern=None):
    if not fromDir or len(fromDir.strip()) == 0:
        return False
    if not toDir or len(toDir.strip()) == 0:
        return False

    if not os.path.isdir(fromDir):
        return False

    if not os.path.isdir(toDir):
        return False

    fromDir = fromDir.replace("\\", "/")
    toDir = toDir.replace("\\", "/")
    externList = None
    if extern:
        externList = extern.split("|")

    for root, dirs, files in os.walk(fromDir, topdown=True):
        for d in dirs:
            ffDir = root+"/"+d;
            tDir = toDir + ffDir.replace(fromDir, "")
            if os.path.isdir(tDir):
                shutil.rmtree(tDir)
            os.mkdir(tDir)

        for f in files:
            canCopy = False
            if externList:
                for e in externList:
                    if f.endswith(e):
                        canCopy = True
                        break
            else:
                canCopy = True
            if canCopy:
                ffPath = root+"/"+f
                tPath = toDir+ffPath.replace(fromDir, "")
                shutil.copy(ffPath, tPath)

    return True


def main():
    # filesysnc(r"E:\Workspace\ZHJWorkspace\Client\DotGameLua\LuaScripts", r"D:\t", ".lua|.txt")
    result = False
    if len(sys.argv) == 3:
        result = filesysnc(sys.argv[1], sys.argv[2])
    if len(sys.argv) == 4:
        result = filesysnc(sys.argv[1], sys.argv[2], sys.argv[3])
    if result:
        print("File Sysnc Success!")
    else:
        print("File Sysnc Failed!")

    print("print any key to exit")
    os.system("pause")


if __name__ == "__main__":
    main()

