#ifndef PROTAGONIST_H
#define PROTAGONIST_H

#include "globals.h"
#include "living_entity.h"

struct Protagonist : Living_Entity {
	// animation names
	static const char* a_idle_up;
	static const char* a_idle_down;
	static const char* a_idle_left;
	static const char* a_idle_right;
	static const char* a_walk_up;
	static const char* a_walk_down;
	static const char* a_walk_left;
	static const char* a_walk_right;
	static const char* a_dash_up;
	static const char* a_dash_down;
	static const char* a_dash_left;
	static const char* a_dash_right;
	static const char* a_attack_up;
	static const char* a_attack_down;
	static const char* a_attack_left;
	static const char* a_attack_right;
	static const char* a_death;

	Protagonist(Animation_Set* animation_set);

	void update();
	void attack();
	void dash();
	void death();
	void resurrect();
	void animate(Entity_State state, int force_first_frame);
	void update_animation();
	void update_dealt_damage();
};

#endif