#ifndef LIVING_ENTITY_H
#define LIVING_ENTITY_H

#include "entity.h"

struct Living_Entity : Entity {
	int current_health;
	int max_health;
	int damage;
	SDL_Rect hitbox;
	float invincible_timer; /* unit: second */

	void render();
	virtual void update_invincible_timer();
	virtual void update_hitbox();
	virtual void update_dealt_damage() = 0;
	virtual void death() = 0;
};

#endif