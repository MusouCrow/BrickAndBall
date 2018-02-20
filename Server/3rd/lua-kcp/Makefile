.PHONY: all lua53 build_dir clean

TOP=$(PWD)
BUILD_DIR=$(TOP)/build
BIN_DIR=$(TOP)/bin
INCLUDE_DIR=$(TOP)/include
LUA_EX_LIB=$(TOP)/lualib
EXTERNAL=$(TOP)/3rd
SRC=$(TOP)/src

CFLAGS = -g3 -O2 -rdynamic -Wall -I$(INCLUDE_DIR) 
SHARED = -fPIC --shared
LDFLAGS = -L$(BUILD_DIR) -Wl,-rpath $(BUILD_DIR)

all: build_dir

build_dir:
	-mkdir $(BUILD_DIR)
	-mkdir $(BIN_DIR)
	-mkdir $(INCLUDE_DIR)
	-mkdir $(LUA_EX_LIB)

all: lua53

lua53:
	cd $(EXTERNAL)/lua && $(MAKE) clean && $(MAKE) MYCFLAGS="-O2 -fPIC -g" linux
	install -p -m 0755 $(EXTERNAL)/lua/src/lua $(BIN_DIR)/lua
	install -p -m 0755 $(EXTERNAL)/lua/src/luac $(BIN_DIR)/luac
	install -p -m 0644 $(EXTERNAL)/lua/src/liblua.a $(BUILD_DIR)
	install -p -m 0644 $(EXTERNAL)/lua/src/*.h $(INCLUDE_DIR)

all: $(LUA_EX_LIB)/lutil.so $(LUA_EX_LIB)/lkcp.so

$(LUA_EX_LIB)/lutil.so: $(SRC)/lutil.c
	gcc $(CFLAGS) $(SHARED) $^ -o $@ 

$(LUA_EX_LIB)/lkcp.so: $(SRC)/lkcp.c  $(EXTERNAL)/kcp/ikcp.c
	cp $(EXTERNAL)/kcp/ikcp.h $(INCLUDE_DIR)
	gcc $(CFLAGS) $(SHARED) $^ -o $@ $(LDFLAGS)

all:
	-rm -rf $(TOP)/*.a $(TOP)/*.o
	@echo 'make finish!!!!!!!!!!!!!!!!!'

clean:
	-rm -rf *.o *.a $(BIN_DIR) $(BUILD_DIR) $(INCLUDE_DIR) $(LUA_EX_LIB)
