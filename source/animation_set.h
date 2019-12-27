#ifndef ANIMATION_SET_H
#define ANIMATION_SET_H

#include <string.h>
#include <SDL_image.h>
#include "animation.h"
#include "globals.h"

struct Animation_Set {
	char* sprite_sheet_name;
	SDL_Texture* sprite_sheet;
	Animation* animations;
	int animation_count;

	~Animation_Set();

	Animation* get_animation(const char* animation_name);
	void load(char* animation_set_file_name);
};

#endif