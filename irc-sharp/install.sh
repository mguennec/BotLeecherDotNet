#!/bin/bash

if [ "$USER" != "root" ]; then
    echo "You need root priveledges to install irc-sharp. Switching..."
    sudo $0 --autosudo
    exit
else
    if [ "$1" == "--autosudo" ]; then
        echo "You are now running as root."
	echo
    fi
fi

monopath=`pkg-config --variable=prefix mono`/bin
monodocpath=`pkg-config --variable=prefix monodoc`

echo "Installing irc-sharp assembly into gac..."

$monopath/gacutil -i bin/ircsharp.dll &> /dev/null

if [ "$monodocpath" = "" ]; then
    echo "Not installing documentation: Monodoc is not installed."
else
    echo "Installing irc-sharp documentation..."
    monodocsourcepath="$monodocpath/lib/monodoc/sources"
    cp docs/ircsharp-docs.* $monodocsourcepath
fi

echo "Installation complete!"
