#!/bin/bash

# Проверка на наличие хотя бы трех аргументов
if [ "$#" -lt 3 ]; then
    echo "Usage: $0 <directory_path> <archive_name> <file1> [file2 ... fileN]"
    exit 1
fi

# Первый аргумент - путь к директории, где будет создан архив (папка storage)
directory_path=$1
# Второй аргумент - имя архива
archive_name=$2

# Удаляем первые два аргумента, чтобы остались только файлы
shift 2

# Создаем путь для архива
archive_path="${directory_path}/${archive_name}"

# Удаляем прошлый zip архив который существовал до этого
rm "$archive_path"

# Создаем zip архив с указанными файлами
zip "$archive_path" "$@"

echo "Archive created at $archive_path with files: $@"