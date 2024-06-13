#pragma once

#include <stdint.h>

enum {
    CPU_HALT = 1
};

typedef struct {
    uint8_t memory[65536];
    uint16_t PC;

    uint16_t register_a;
    uint16_t register_x;
    uint16_t register_y;

    uint8_t state;

    uint16_t addressBus;
    uint16_t dataBus;
} CPU;

void CPU_Init(CPU* cpu, uint8_t* rom);
int CPU_Execute(CPU* cpu);