# Nyvorn Reborn Roadmap

## Visao do Projeto

`Nyvorn Reborn` sera um jogo de combate focado em boss fights por sala.

Pilares do projeto:

- Uma sala por combate
- Um boss por sala
- Camera fixa na sala
- Progressao por aprendizado + moedas
- Recompensa mesmo na derrota
- Foco inicial em uma unica classe: `Guerreiro`

Objetivo de produto:

- Entregar um jogo menor, fechavel e polido
- Validar combate, leitura de padroes e progressao curta
- Usar essa base como versao jogavel antes de expandir para projetos maiores

## Versao Atual

### `v0.1.0` - Base Jogavel

Estado atual da base:

- Player funcional
- Movimento, pulo, dodge e ataque
- Sistema de combate modular inicial
- Inimigo funcional para testes
- Inventario e hotbar basicos
- State stack implementado
- HUD inicial
- Itens no mundo com gravidade

Essa versao representa a fundacao tecnica inicial antes do corte definitivo para o formato de boss-rush.

## Roadmap de Versoes

### `v0.2.0` - Fundacao do Reborn

Objetivo:

- Transformar a base atual em um projeto focado no formato `boss room`

Entregas:

- Duplicar projeto para o repositorio `Nyvorn Reborn`
- Ajustar escopo para uma experiencia menor e fechada
- Camera fixa na sala, sem follow do player
- Definir arena principal unica
- Remover ou despriorizar sistemas que nao servem ao boss-rush imediato
- Consolidar o loop central:
  - entrar na sala
  - lutar
  - morrer ou vencer

### `v0.3.0` - Primeiro Boss Loop

Objetivo:

- Criar a primeira luta real contra boss

Entregas:

- Primeiro boss funcional
- Vida, hurtbox e hitbox do boss
- Padroes simples de ataque
- Telegraph visual minimo
- Dano no player e morte do player
- Derrota do boss
- Reset limpo da luta

### `v0.4.0` - Progressao por Moedas

Objetivo:

- Fazer cada tentativa gerar progresso

Entregas:

- Sistema de moedas
- Ganho de moedas na derrota
- Ganho de moedas na vitoria
- Tela simples de resultado pos-luta
- Persistencia simples das moedas entre tentativas

### `v0.5.0` - Equipamentos do Guerreiro

Objetivo:

- Dar progressao real ao player

Entregas:

- Equipamento real vindo da hotbar
- Primeiras armas do guerreiro
- Armaduras ou upgrades simples
- Loja ou tela de compra simples
- Integracao entre moedas e equipamento

### `v0.6.0` - Polimento do Combate

Objetivo:

- Melhorar game feel e leitura das lutas

Entregas:

- Refinar dodge
- Refinar timings de ataque
- Refinar hitbox e hurtbox
- Refinar knockback
- Feedback visual de dano
- Feedback visual de ataque do boss
- Balanceamento da luta

### `v0.7.0` - Vertical Slice

Objetivo:

- Fechar uma versao curta, completa e testavel

Entregas:

- 1 sala
- 1 boss
- 1 classe jogavel: `Guerreiro`
- 1 ciclo completo de luta, derrota, recompensa e melhoria
- Tela de morte
- Tela de vitoria
- Fluxo coeso do inicio ao fim

### `v0.8.0` - Expansao de Conteudo

Objetivo:

- Comecar a expandir o jogo sem mudar sua identidade

Entregas:

- Segundo boss
- Segunda sala
- Mais equipamentos
- Mais recompensas
- Mais variedade de combate

### `v0.9.0` - Estrutura para Escala

Objetivo:

- Preparar a base para crescer com seguranca

Entregas:

- Mais dados em configs
- Melhor organizacao de bosses
- Melhor organizacao de progressao
- Preparacao para novas classes no futuro
- Refinos estruturais finais antes do release

### `v1.0.0` - Primeiro Release Fechado

Objetivo:

- Entregar a primeira versao completa de `Nyvorn Reborn`

Entregas:

- Loop principal consolidado
- Progressao funcional
- Bosses e salas suficientes para sustentar a proposta
- Combate polido
- Interface minima solida
- Estrutura pronta para atualizacoes futuras

## Decisoes de Escopo

Decisoes aprovadas para manter o projeto controlavel:

- Comecar apenas com `Guerreiro`
- Nao implementar `Mago` no inicio
- Camera fixa por sala
- Prioridade total no combate contra boss
- Um jogo menor primeiro, antes de retomar o projeto maior

## Ordem Recomendada de Producao

1. Fechar `v0.2.0`
2. Construir o primeiro boss em `v0.3.0`
3. Fechar moedas e recompensa em `v0.4.0`
4. Fechar progressao do guerreiro em `v0.5.0`
5. Polir combate em `v0.6.0`
6. Fechar vertical slice em `v0.7.0`

## Tags Recomendadas

- `v0.1.0` Base jogavel inicial
- `v0.2.0` Fundacao do Reborn
- `v0.3.0` Primeiro boss loop
- `v0.4.0` Progressao por moedas
- `v0.5.0` Equipamentos do guerreiro
- `v0.6.0` Polimento do combate
- `v0.7.0` Vertical slice
- `v0.8.0` Expansao de conteudo
- `v0.9.0` Estrutura para escala
- `v1.0.0` Primeiro release fechado
