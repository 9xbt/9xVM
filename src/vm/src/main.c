#include <SDL2/SDL.h>
#include <stdio.h>
#include <string.h>
#include <stdbool.h>
#include "cpu.h"
#include "tvo.h"

SDL_Window* window = NULL;
SDL_Renderer* renderer = NULL;
CPU cpu;

void SDL_Update() {
    SDL_Event e;
    while (!(cpu.state & 1)) {
        if (CPU_Execute(&cpu)) {
            printf("Generic CPU Error\n");

            SDL_DestroyRenderer(renderer);
            SDL_DestroyWindow(window);
            SDL_Quit();

            exit(1);
        }

        TVO_Render(renderer, &cpu);
        SDL_RenderPresent(renderer);

        while (SDL_PollEvent(&e)) {
            if (e.type == SDL_QUIT) {
                SDL_DestroyRenderer(renderer);
                SDL_DestroyWindow(window);
                SDL_Quit();

                exit(0);
            }
        }
    }
}

int main(int argc, char **argv) {
    if (SDL_Init(SDL_INIT_VIDEO) < 0) {
        printf("SDL Error: %s\n", SDL_GetError());
        return 1;
    }

    window = SDL_CreateWindow("9xVM", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, 640, 400, SDL_WINDOW_SHOWN);
    renderer = SDL_CreateRenderer(window, -1, 0);

    SDL_SetRenderDrawColor(renderer, 0, 0, 0, 0);
    SDL_RenderClear(renderer);
    SDL_RenderPresent(renderer);

    if (window == NULL) {
        printf("Couldn't initialize main window: %s\n", SDL_GetError());
        return 1;
    }

    FILE* file = fopen(argv[1], "rb");
    uint8_t rom[0x8000];

    if (file == NULL) {
        printf("IO Error: failed to open rom\n");
        return 1;
    }

    fread(rom, sizeof(rom), 1, file);
    fclose(file);

    CPU_Init(&cpu, rom);
    SDL_Update();

    return 0;
}