#ifndef WALL_H
#define WALL_H

#include "entity.h"

struct Wall : Entity {
	static const char* a_idle;

	Wall(Animation_Set* animation_set);

	void update();
	void animate(Entity_State state, int force_first_frame);
	void update_collisions();
};

#endif