#include "animation_set.h"

Animation_Set::~Animation_Set() {
	if (sprite_sheet_name) delete[] sprite_sheet_name;
	if (sprite_sheet) SDL_DestroyTexture(sprite_sheet);
	if (animations) delete[] animations;
}

Animation* Animation_Set::get_animation(char* animation_name) {
	for (int i = 0; i < animation_count; i++)
		if (strcmp(animation_name, (animations + i)->name) == 0) return animations + i;
	return nullptr;
}

void Animation_Set::load(char* animation_set_file_name) {
	FILE* stream;
	stream = fopen(animation_set_file_name, "r");
	if (stream) {
		char buffer[ANIMATION_DATA_FILE_PARSING_BUFFER];
		int next_animation_to_process_index = 0;

		/* sprite_sheet_file_name %d %s */
		const char* str_sprite_sheet_file_name = "sprite_sheet_file_name";
		/* animation_count %d */
		const char* str_animation_count = "animation_count";
		/* animation_start */
		const char* str_animation_start = "animation_start";

		while (fgets(buffer, sizeof(buffer), stream)) {
			if (strncmp(buffer, str_sprite_sheet_file_name, sizeof(str_sprite_sheet_file_name))) {
				int sprite_sheet_file_name_length = 0;
				sscanf(buffer, "sprite_sheet_file_name %d ", &sprite_sheet_file_name_length);
				if (sprite_sheet_name) delete[] sprite_sheet_name;
				sprite_sheet_name = new char[sprite_sheet_file_name_length + 1];
				sscanf(buffer, "sprite_sheet_file_name %d %s", &sprite_sheet_file_name_length, sprite_sheet_name);
				if (sprite_sheet) SDL_DestroyTexture(sprite_sheet);
				sprite_sheet = IMG_LoadTexture(Globals::renderer, Globals::get_resource_path(sprite_sheet_name));
			}
			else if (strncmp(buffer, str_animation_count, sizeof(str_animation_count))) {
				sscanf(buffer, "animation_count %d", &animation_count);
				if (animations) delete[] animations;
				animations = new Animation[animation_count];
			}
			else if (strncmp(buffer, str_animation_start, sizeof(str_animation_start))) {
				int name_length, frame_count;
				sscanf(buffer, "animation_start name_length %d frame_count %d", &name_length, &frame_count);
				animations[next_animation_to_process_index++].load(stream, name_length, frame_count);
			}
		}

		fclose(stream);
	}
}