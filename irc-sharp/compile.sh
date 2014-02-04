#!/bin/bash

if [ ! -d "docs" ]
then
    mkdir "docs"
fi

if [ ! -d "bin" ]
then
    mkdir "bin"
fi

echo Compiling code...
gmcs ./src/*.cs ./src/Users/*.cs ./src/Misc/*.cs ./src/Channels/*.cs ./src/DCC/*.cs ./src/ServerInfo/*.cs -target:library -out:./bin/ircsharp.dll -keyfile:./src/IRC.key -doc:./docs/ircsharp.xml > /dev/null

echo Generating monodoc documentation...
monodocer -name:"ircsharp" -importslashdoc:./docs/ircsharp.xml -path:./docs/en -assembly:./bin/ircsharp.dll -pretty > /dev/null

echo Generating html documentation...
monodocs2html --source ./docs/en --dest ./docs/html > /dev/null

cd docs

echo Generating monodoc assembly...
mdassembler --ecma en -o ircsharp-docs &> /dev/null

echo "<?xml version=\"1.0\"?>" > ircsharp-docs.source
echo "<monodoc>" >> ircsharp-docs.source
echo "    <source provider=\"ecma\" basefile=\"ircsharp-docs\" path=\"various\"/>" >> ircsharp-docs.source
echo "</monodoc>" >> ircsharp-docs.source

rm -R en

cd ../

echo
echo All tasks complete
echo
echo ./bin contains binaries
echo ./docs contains documentation
echo
echo Run install.sh to install assembly and documentation.
echo
echo Have fun!
