#include "keyboard_input.h"

Keyboard_Input::Keyboard_Input() {
	up = SDL_SCANCODE_UP;
	down = SDL_SCANCODE_DOWN;
	left = SDL_SCANCODE_LEFT;
	right = SDL_SCANCODE_RIGHT;
	attack = SDL_SCANCODE_A;
	dash = SDL_SCANCODE_D;
}

void Keyboard_Input::update(SDL_Event* event) {
	if (event->type == SDL_KEYDOWN) {
		if (event->key.keysym.scancode == dash) protagonist->dash();
		if (event->key.keysym.scancode == attack) protagonist->attack();
	}
	const Uint8* key_states = SDL_GetKeyboardState(nullptr);
	if ((protagonist->state != Entity::STATE_WALKING && protagonist->state != Entity::STATE_IDLING)
		|| (!key_states[up] && !key_states[down] && !key_states[left] && !key_states[right])) {
		protagonist->is_moving = false;
	}
	else {
		if (key_states[up]) {
			if (key_states[right]) protagonist->move(315);
			else if (key_states[left]) protagonist->move(225);
			else protagonist->move(270);
		}
		if (key_states[down]) {
			if (key_states[right]) protagonist->move(45);
			else if (key_states[left]) protagonist->move(135);
			else protagonist->move(90);
		}
		if (!key_states[up] && !key_states[down] && key_states[left] && !key_states[right]) protagonist->move(180);
		if (!key_states[up] && !key_states[down] && !key_states[left] && key_states[right]) protagonist->move(0);
	}
}