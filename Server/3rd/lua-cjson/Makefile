OUTPUT_DIR = ../../lib
PLATFORM = $(shell sh -c 'uname -s 2>/dev/null || echo not')

ifeq ($(PLATFORM), Darwin)
SHARED = -fPIC
CFLAGS = -O3 -Wall -pedantic -DNDEBUG
BUNDLE = -bundle -undefined dynamic_lookup
else
SHARED = -fPIC --shared
CFLAGS = -g -O2 -Wall
endif

INCLUDE = -I../../skynet/3rd/lua
OBJS = lua_cjson.o strbuf.o fpconv.o
TARGET = $(OUTPUT_DIR)/cjson.so

.PHONY: all clean

.c.o:
	$(CC) -c $(SHARED) $(CFLAGS) $(INCLUDE) $(BUILD_CFLAGS) -o $@ $<

all: $(TARGET)

$(TARGET): $(OBJS)
ifeq ($(PLATFORM), Darwin)
	$(CC) $(LDFLAGS) $(BUNDLE) $^ -o $@
else
	$(CC) $(CFLAGS) $(INCLUDE) $(SHARED) $^ -o $@
endif

clean:
	rm -f *.o
