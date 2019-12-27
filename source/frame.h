#ifndef FRAME_H
#define FRAME_H

#include <stdio.h>
#include <string.h>
#include <SDL.h>
#include "globals.h"

struct Frame {
	int index;
	SDL_Rect clip_zone;
	float duration;
	SDL_Point offset;
	int is_damaging;
	SDL_Rect hitbox;
	int damage;

	void render(SDL_Texture* texture, int x, int y);
	void load(FILE* stream);
};

#endif