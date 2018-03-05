basepath=$(cd `dirname $0`; cd ..; pwd)
platform=$(uname)

function build() {
    echo "====================="
    echo "start build skyent..."
    cd $basepath/skynet

    if [[ "$platform" == "Darwin" ]]; then
        make macosx
    else
        make linux MALLOC_STATICLIB= SKYNET_DEFINES=-DNOUSE_JEMALLOC
    fi

    echo "====================="
    echo "start build lua-kcp..."
    cd $basepath/3rd/lua-kcp
    make

    echo "====================="
    echo "start build lua-cjson..."
    cd $basepath/3rd/lua-cjson
    make

    echo "====================="
    echo "clean..."
    cd $basepath
    find . -name "*.o"  | xargs rm -f

    echo "finish"
}

function clean() {
	echo "====================="
	echo "start clean skyent..."
	cd $basepath/skynet
	git checkout . && git clean -xdf

	echo "====================="
	echo "start clean lualib..."
    cd $basepath
    rm -f lib/*.so
    find . -name "*.dSYM"  | xargs rm -f -r
    find . -name "*.o"  | xargs rm -f

    echo "finish"
}

if [[ "$1" == "" ]]; then
	build
elif [[ "$1" == "clean" ]]; then
	clean
fi
