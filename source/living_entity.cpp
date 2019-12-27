#include "living_entity.h"

void Living_Entity::render() {
	Entity::render();
#ifdef DEBUG
	if (current_frame && current_frame->is_damaging) {
		SDL_SetRenderDrawColor(Globals::renderer, 214, 51, 255, SDL_ALPHA_OPAQUE);
		SDL_RenderDrawRect(Globals::renderer, &hitbox);
	}
#endif
}

void Living_Entity::update_invincible_timer() {
	if (invincible_timer > 0) invincible_timer -= Time_Controller::time_controller.delta_time;
}

void Living_Entity::update_hitbox() {
	if (current_frame && current_frame->is_damaging) {
		hitbox.x = x - current_frame->offset.x + current_frame->hitbox.x;
		hitbox.y = y - current_frame->offset.y + current_frame->hitbox.y;
		hitbox.w = current_frame->hitbox.w;
		hitbox.h = current_frame->hitbox.h;
		damage = current_frame->damage;
	}
}