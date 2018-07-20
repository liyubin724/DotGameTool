#! /usr/bin/env python
# -*- coding: utf-8 -*-

import sys
import os
import shutil
import re


def split_file(file_path, target_dir, max_size):
    if not os.path.isfile(file_path):
        return False
    if not os.path.exists(target_dir):
        os.mkdir(target_dir)
    file_size = os.path.getsize(file_path)
    fpath, fname = os.path.split(file_path)
    print(file_size)
    if file_size <= max_size:
        shutil.copy(file_path, os.path.join(target_dir, fname))
    else:
        with open(file_path, "rb") as file:
            file_index = 0
            short_name, extension = os.path.splitext(fname)
            while True:
                if file_size > 0:
                    file_index += 1
                    temp_file_path = os.path.join(target_dir, short_name+"_"+str(file_index)+extension)
                    write_size = 0
                    with open(temp_file_path, "wb") as new_file:
                        while True:
                            read_data = file.read(256)
                            if len(read_data) == 0:
                                break
                            else:
                                write_size += len(read_data)
                                new_file.write(read_data)
                                if write_size + 256 > max_size:
                                    break
                        new_file.flush()
                    file_size -= write_size
                else:
                    break
    return True


def join_file(file_dir, file, target_file):
    if not file_dir or len(file_dir) == 0 or not os.path.isdir(file_dir):
        return False
    if not os.path.exists(file_dir):
        return False

    files_in_dir = []
    pattern = "^"+file+"_[0-9]+"
    for f in os.listdir(file_dir):
        if os.path.isfile(os.path.join(file_dir,f)):
            file_short_name,file_extension = os.path.splitext(f)
            if re.match(pattern,file_short_name):
                files_in_dir.append(f)

    files_in_dir.sort(key=join_file_sort)

    target_dir, file_name = os.path.split(target_file)
    if not target_dir or len(target_dir) == 0:
        return False
    if not os.path.exists(target_dir):
        os.mkdir(target_dir)

    with open(target_file, "wb") as file:
        for f in files_in_dir:
            file_path = os.path.join(file_dir, f)
            with open(file_path, "rb") as src_file:
                while True:
                    data = src_file.read(256)
                    if len(data) == 0:
                        break
                    else:
                        file.write(data)
            file.flush()
    return True


def join_file_sort(elem):
    file_short_name, file_extension = os.path.splitext(elem)
    index = file_short_name.split("_")[1]
    return int(index)


def main():
    result = False
    if len(sys.argv) == 5:
        action = sys.argv[1]
        if action.lower() == "split":
            result = split_file(sys.argv[2], sys.argv[3], int(sys.argv[4]))
        elif action.lower() == "join":
            result = join_file(sys.argv[2],sys.argv[3],sys.argv[4])
    else:
        result = split_file(r"D:\dd.pdf", "D:\m", 1024*1024)
        # result = join_file(r"D:\t", "t", r"D:\m\t.rar")


if __name__ == "__main__":
    main()
