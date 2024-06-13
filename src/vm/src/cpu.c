#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>
#include <string.h>
#include "cpu.h"

void CPU_Init(CPU* cpu, uint8_t* rom) {
    if (cpu == NULL) {
        printf("CPU Init Error: null pointer\n");
        exit(1);
    }

    memcpy(cpu->memory + 0x8000, rom, 0x8000);
    cpu->PC = 0x8000;
    cpu->state = 0;
}

int CPU_Execute(CPU* cpu) {
    if (cpu->PC == 0xffff) {
        printf("CPU Error: invalid address\n");
        cpu->state |= CPU_HALT;
        return 1;
    }

    uint16_t opcode;
    memcpy(&opcode, cpu->memory + cpu->PC, 2);

    printf("PC: %04x\n", cpu->PC);
    printf("Opcode: %04x\n", opcode);

    switch (opcode) {
        case 0x0000:
            cpu->PC += 2;
            break;

        case 0x0001:
            /* JMP [addr] */
            memcpy(&cpu->addressBus, cpu->memory + cpu->PC + 2, 2);

            cpu->PC = cpu->addressBus;
            break;

        case 0x0002:
            /* JMP [reg] */
            memcpy(&cpu->dataBus, cpu->memory + cpu->PC + 2, 2);
            memcpy(&cpu->addressBus, &cpu->regs[cpu->dataBus], 2);

            cpu->PC = cpu->addressBus;
            break;

        case 0x0010:
            /* MOV [addr] [val] */
            memcpy(&cpu->addressBus, cpu->memory + cpu->PC + 2, 2);
            memcpy(&cpu->dataBus, cpu->memory + cpu->PC + 4, 2);
            memcpy(cpu->memory + cpu->addressBus, &cpu->dataBus, 2);

            cpu->PC += 6;
            break;

        case 0x0011:
            /* MOV [addr] [reg] */
            memcpy(&cpu->addressBus, cpu->memory + cpu->PC + 2, 2);
            memcpy(&cpu->registerBus, cpu->memory + cpu->PC + 4, 2);
            memcpy(cpu->memory + cpu->addressBus, &cpu->regs[cpu->registerBus - 1], 2);

            cpu->PC += 6;
            break;

        case 0x0012:
            /* MOV [reg] [val] */
            memcpy(&cpu->registerBus, cpu->memory + cpu->PC + 2, 2);
            memcpy(&cpu->dataBus, cpu->memory + cpu->PC + 4, 2);
            memcpy(&cpu->regs[cpu->registerBus - 1], &cpu->dataBus, 2);

            cpu->PC += 6;
            break;

        case 0x0013:
            /* MOV [reg] [reg] */
            memcpy(&cpu->registerBus, cpu->memory + cpu->PC + 2, 2);
            cpu->addressBus = cpu->regs[cpu->registerBus - 1];
            memcpy(&cpu->registerBus, cpu->memory + cpu->PC + 4, 2);
            memcpy(cpu->memory + cpu->addressBus, &cpu->registerBus, 2);

            cpu->PC += 6;
            break;

        case 0xFFFF:
            cpu->state = CPU_HALT;

            printf("CPU Halted!\n");
            return 0;

        default:
            cpu->state = CPU_HALT;

            printf("CPU Error: invalid opcode\n");
            return 0;
    }

    printf("Address bus: %04x\n", cpu->addressBus);
    printf("Data bus: %04x\n", cpu->dataBus);
    printf("-----------------\n");

    return 0;
}