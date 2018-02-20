#include <lua.h>
#include <lualib.h>
#include <lauxlib.h>

#include <stdint.h>
#include <stdlib.h>

#if defined(WIN32) || defined(_WIN32) || defined(WIN64) || defined(_WIN64)
#include <windows.h>
#elif !defined(__unix)
#define __unix
#endif

#ifdef __unix
#include <unistd.h>
#include <sys/time.h>
#include <sys/wait.h>
#include <sys/types.h>
#endif

static int _l_gettimeofday(lua_State * L){
    struct timeval tv;
    gettimeofday(&tv, NULL);

    uint32_t r = (uint32_t)(((uint64_t)tv.tv_sec * 1000 + (uint64_t)tv.tv_usec / 1000) & 0x7ffffffful);
    lua_pushinteger(L, r);
    return 1;
}

/* sleep in millisecond */
static int _l_isleep(lua_State* L)
{
    uint32_t millisecond = luaL_checkinteger(L, 1);
	#ifdef __unix
	usleep((millisecond << 10) - (millisecond << 4) - (millisecond << 3));
	#elif defined(_WIN32)
	Sleep(millisecond);
	#endif
    return 0;
}

static int _netbytes2uint32(lua_State* L){
    const char* s = luaL_checkstring(L, 1);
    uint32_t a0 = (uint8_t)s[0] << 24;
    uint32_t a1 = (uint8_t)s[1] << 16;
    uint32_t a2 = (uint8_t)s[2] << 8;
    uint32_t a3 = (uint8_t)s[3];
    uint32_t ret = (uint32_t)(a0 | a1 | a2 | a3);
    lua_pushinteger(L, ret);
    return 1;
}

static int _uint322netbytes(lua_State* L){
    uint32_t i = lua_tointeger(L, 1);
    char s[4];
    s[0] = (char)(i >> 24);
    s[1] = (char)(i >> 16);
    s[2] = (char)(i >> 8);
    s[3] = (char)(i);
    lua_pushlstring(L, s, 4);
    return 1;
}

int luaopen_lutil(lua_State *L) {
    luaL_Reg libs[] = {
        {"gettimeofday", _l_gettimeofday},
        {"isleep", _l_isleep},
        {"netbytes2uint32", _netbytes2uint32},
        {"uint322netbytes", _uint322netbytes},
        {NULL, NULL},
    };

    luaL_newlib(L, libs);
    return 1;
}

