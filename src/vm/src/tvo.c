#include <SDL2/SDL.h>
#include <stddef.h>
#include "cpu.h"
#include "tvo.h"

void TVO_WriteChar(SDL_Renderer* renderer, char c, int x, int y) {
    if (renderer == NULL) {
        printf("TVO Error: null pointer\n");
        exit(1);
    }

    for (int i = 0; i < TVO_FONT_HEIGHT; i++) {
        for (int o = 0; o < TVO_FONT_WIDTH; o++) {
            if (tvo_font[c * TVO_FONT_HEIGHT + i] & (1 << (7 - o))) {
                SDL_RenderDrawPoint(renderer, x + o, y + i);
            }
        }
    }
}

void TVO_Render(SDL_Renderer* renderer, CPU* cpu) {
    SDL_SetRenderDrawColor(renderer, 0, 0, 0, 0);
    SDL_RenderClear(renderer);
    SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
    for (int y = 0; y < TVO_HEIGHT; y++) {
        for (int x = 0; x < TVO_WIDTH; x++) {
            TVO_WriteChar(renderer, cpu->memory[TVO_ADDR + (y * TVO_WIDTH + x)], x * TVO_FONT_WIDTH, y * TVO_FONT_HEIGHT);
        }
    }
}