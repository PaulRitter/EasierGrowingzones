#!/bin/bash

mkdir -p EasierGrowingzones
cp -r About Languages EasierGrowingzones/
sed -i "/<supportedVersions>/ a \ \ \ \ <li>1.2</li>" EasierGrowingzones/About/About.xml
sed -i "/<supportedVersions>/ a \ \ \ \ <li>1.1</li>" EasierGrowingzones/About/About.xml
sed -i "/<supportedVersions>/ a \ \ \ \ <li>1.0</li>" EasierGrowingzones/About/About.xml

rm -rf EasierGrowingzones/1.3
mkdir -p EasierGrowingzones/1.3
cp -r Assemblies Defs EasierGrowingzones/1.3/

mkdir -p EasierGrowingzones/1.2
git --work-tree=EasierGrowingzones/1.2 checkout origin/rw-1.2 -- Assemblies Defs
git reset Assemblies Defs

mkdir -p EasierGrowingzones/1.1
git --work-tree=EasierGrowingzones/1.1 checkout origin/rw-1.2 -- Assemblies Defs
git reset Assemblies Defs

mkdir -p EasierGrowingzones/1.0
git --work-tree=EasierGrowingzones/1.0 checkout origin/rw-1.2 -- Assemblies Defs
git reset Assemblies Defs

rm -f EasierGrowingzones.zip
7z a -tzip EasierGrowingzones.zip EasierGrowingzones

echo "Ok, $PWD/EasierGrowingzones.zip ready for uploading to Workshop"