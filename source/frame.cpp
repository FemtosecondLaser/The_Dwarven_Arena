#include "frame.h"

void Frame::render(SDL_Texture* texture, int x, int y) {
	SDL_Rect destination;
	destination.x = x - offset.x;
	destination.y = y - offset.y;
	destination.w = clip_zone.w;
	destination.h = clip_zone.h;

	SDL_RenderCopy(Globals::renderer, texture, &clip_zone, &destination);
}

void Frame::load(FILE* stream) {
	char buffer[ANIMATION_DATA_FILE_PARSING_BUFFER];

	/* index %d */
	const char* str_index = "index";
	/* clip_zone %d %d %d %d */
	const char* str_clip_zone = "clip_zone";
	/* duration	 %f */
	const char* str_duration = "duration";
	/* offset %d %d */
	const char* str_offset = "offset";
	/* is_damaging %d */
	const char* str_is_damaging = "is_damaging";
	/* hitbox %d %d %d %d */
	const char* str_hitbox = "hitbox";
	/* damage %d */
	const char* str_damage = "damage";
	/* frame_end */
	const char* str_frame_end = "frame_end";

	while (fgets(buffer, sizeof(buffer), stream)) {
		if (strncmp(buffer, str_index, sizeof(str_index)))
			sscanf(buffer, "index %d", &index);
		else if (strncmp(buffer, str_clip_zone, sizeof(str_clip_zone)))
			sscanf(buffer, "clip_zone %d %d %d %d", &(clip_zone.x), &(clip_zone.y), &(clip_zone.w), &(clip_zone.h));
		else if (strncmp(buffer, str_duration, sizeof(str_duration)))
			sscanf(buffer, "duration %f", &duration);
		else if (strncmp(buffer, str_offset, sizeof(str_offset)))
			sscanf(buffer, "offset %d %d", &(offset.x), &(offset.y));
		else if (strncmp(buffer, str_is_damaging, sizeof(str_is_damaging)))
			sscanf(buffer, "is_damaging %d", &is_damaging);
		else if (strncmp(buffer, str_hitbox, sizeof(str_hitbox)))
			sscanf(buffer, "hitbox %d %d %d %d", &(hitbox.x), &(hitbox.y), &(hitbox.w), &(hitbox.h));
		else if (strncmp(buffer, str_damage, sizeof(str_damage)))
			sscanf(buffer, "damage %d", &damage);
		else if (strncmp(buffer, str_frame_end, sizeof(str_frame_end))) return;
	}
}