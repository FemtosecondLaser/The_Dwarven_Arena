#ifndef KEYBOARD_INPUT_H
#define KEYBOARD_INPUT_H

#include "protagonist.h"

struct Keyboard_Input {
	Protagonist* protagonist;
	SDL_Scancode up, down, left, right, dash, attack;

	Keyboard_Input();

	void update(SDL_Event* event);
};

#endif