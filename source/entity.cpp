#include "entity.h"

Entity** Entity::entities;
int Entity::entity_count;

void Entity::update() {
	// NOOP
}

void Entity::render() {
	if (current_frame && is_active) {
		current_frame->render(animation_set->sprite_sheet, x, y);
	}
#ifdef DEBUG
	if (!can_be_passed_through) {
		SDL_SetRenderDrawColor(Globals::renderer, 204, 0, 102, SDL_ALPHA_OPAQUE);
		SDL_RenderDrawRect(Globals::renderer, &current_collision_box);
	}
#endif
}

void Entity::move(float angle) {
	is_moving = 1;
	move_speed = move_speed_upper_limit;
	this->angle = angle;
	Entity_Move_Direction new_direction = angle_to_direction(this->angle);
	if (move_direction != new_direction) {
		move_direction = new_direction;
		animate(state, 0);
	}
}

void Entity::update_movement() {
	update_collision_box();
	last_collision_box = current_collision_box;
	if (move_speed > 0) {
		float move_distance = move_speed * Time_Controller::time_controller.delta_time * move_lerp;
		if (move_distance > 0) {
			float x_move = move_distance * (cos(angle * PI / 180));
			float y_move = move_distance * (sin(angle * PI / 180));
			x += x_move;
			y += y_move;
			if (!is_moving) move_speed -= move_distance;
		}
	}
	if (slide_amount > 0) {
		float slide_distance = slide_amount * Time_Controller::time_controller.delta_time * move_lerp;
		if (slide_distance > 0) {
			float x_move = slide_distance * (cos(slide_angle * PI / 180));
			float y_move = slide_distance * (sin(slide_angle * PI / 180));
			x += x_move;
			y += y_move;
			slide_amount -= slide_distance;
		}
		else slide_amount = 0;
	}
	update_collision_box();
}

void Entity::update_collision_box() {
	current_collision_box.x = x - current_collision_box.w / 2;
	current_collision_box.y = y + collision_box_vertical_offset;
	current_collision_box.w = collision_box_width;
	current_collision_box.h = collision_box_height;
}

void Entity::update_collisions() {
	SDL_UnionRect(&current_collision_box, &last_collision_box, &union_current_last_collision_box);
	update_collision_box();
	if (is_active && can_collide_with_non_pass_through_entities) {
		for (int i = 0; i < Entity::entity_count; i++) {
			if ((entities + i)
				&& (*(entities + i))->is_active
				&& (*(entities + i))->label != this->label
				&& !(*(entities + i))->can_be_passed_through
				&& Entity::is_colliding(&current_collision_box, &(*(entities + i))->current_collision_box)) {
				float checking_distance, movement_angle;
				if (current_collision_box.w < current_collision_box.h) checking_distance = current_collision_box.w / 4;
				else checking_distance = current_collision_box.h / 4;
				SDL_Rect sample_collision_box = last_collision_box;
				movement_angle = angle_between_rectangles(&last_collision_box, &current_collision_box);
				int found_collision = 0;
				while (!found_collision) {
					SDL_Rect intersection;
					if (SDL_IntersectRect(&sample_collision_box, &(*(entities + i))->current_collision_box, &intersection)) {
						found_collision = 0;
						move_speed = 0;
						is_moving = 0;
						slide_angle = angle_between_entities((*(entities + i)), this);
						if (intersection.w < intersection.h) {
							if (last_collision_box.x + last_collision_box.w / 2 < (*(entities + i))->current_collision_box.x + (*(entities + i))->current_collision_box.w / 2) sample_collision_box.x -= intersection.w;
							else sample_collision_box.x += intersection.w;
						}
						else {
							if (last_collision_box.y + last_collision_box.h / 2 < (*(entities + i))->current_collision_box.y + (*(entities + i))->current_collision_box.h / 2) sample_collision_box.y -= intersection.h;
							else sample_collision_box.y += intersection.h;
						}
					}
					if (found_collision || (sample_collision_box.x == current_collision_box.x && sample_collision_box.y == current_collision_box.y)) break;
					if (distance_between_rectangles(&sample_collision_box, &current_collision_box) > checking_distance) {
						float x_move = checking_distance * (cos(movement_angle * PI / 180));
						float y_move = checking_distance * (sin(movement_angle * PI / 180));
						sample_collision_box.x += x_move;
						sample_collision_box.y += y_move;
					}
					else {
						sample_collision_box = current_collision_box;
					}
				}
				if (found_collision) {
					slide_amount /= 2;
					x = sample_collision_box.x + sample_collision_box.w / 2;
					y = sample_collision_box.y + collision_box_vertical_offset;
				}
				update_collision_box();
			}
		}
	}
}

float Entity::distance_between_rectangles(SDL_Rect* first, SDL_Rect* second) {
	return abs(sqrt(pow(second->x + second->w / 2 - first->x + first->w / 2, 2) + pow(second->y + second->h / 2 - first->y + first->h / 2, 2)));
}

float Entity::distance_between_entities(Entity* first, Entity* second) {
	return abs(sqrt(pow(second->x - first->x, 2) + pow(second->y - first->y, 2)));
}

float Entity::angle_between_entities(Entity* first, Entity* second) {
	return atan2(second->y - first->y, second->x - first->x) * 180 / PI;
}

int Entity::is_colliding(SDL_Rect* first, SDL_Rect* second) {
	return SDL_IntersectRect(first, second, nullptr);
}

Entity::Entity_Move_Direction Entity::angle_to_direction(float angle) {
	if ((angle >= 0 && angle <= 45) || (angle >= 315 && angle <= 360)) return DIRECTION_RIGHT;
	else if (angle >= 45 && angle <= 135) return DIRECTION_DOWN;
	else if (angle >= 135 && angle <= 225) return DIRECTION_LEFT;
	else return DIRECTION_UP;
}

float Entity::angle_between_points(float first_x, float first_y, float second_x, float second_y) {
	return abs(sqrt(pow(second_x - first_x, 2) + pow(second_y - first_y, 2)));
}

float Entity::angle_between_rectangles(SDL_Rect* first, SDL_Rect* second) {
	return angle_between_points(first->x + first->w / 2, first->y + first->h / 2, first->x + first->w / 2, first->y + first->h / 2);
}

int Entity::compare_entities(Entity* first, Entity* second) {
	if (first && second) return first->y < second->y;
	else return 0;
}

void Entity::remove_inactive_entities(Entity** entities, int entity_count, int delete_inactive) {
	for (int i = 0; i < entity_count; i++) {
		if ((entities + i) && !(*(entities + i))->is_active) {
			if (delete_inactive) delete* (entities + i);
			*(entities + i) = nullptr;
		}
	}
}

void Entity::remove_entities(Entity** entities, int entity_count, int delete_inactive) {
	for (int i = 0; i < entity_count; i++) {
		if (entities + i) {
			if (delete_inactive) delete* (entities + i);
			*(entities + i) = nullptr;
		}
	}
}