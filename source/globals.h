#ifndef GLOBALS_H
#define GLOBALS_H

#include <limits.h>
#include <string.h>
#include <SDL.h>

#define PI 3.14159265f

/* stringification */
#define TO_STRING(x) STRING(x)
#define STRING(x) #x

#define ANIMATION_DATA_FILE_PARSING_BUFFER 128

#define RESOURCES_DIR_RELATIVE_PATH "resources/"

struct Globals {
	static int screen_width, screen_height, screen_scale;
	static SDL_Renderer* renderer;

	static int digit_count(int i);
	static char* get_resource_path(char* resource_name);
};

#endif