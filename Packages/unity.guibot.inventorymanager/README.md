# Guibot - Unity Inventory Manager

Gestor de inventario para 2D e 3D. O nucleo (`ItemData`, `Inventory`) nao sabe nada de fisica nem de UI; so os pickups e a UI mudam entre 2D e 3D.

## Instalar (alunos)

1. Descarrega o ZIP da pagina de **Releases** do GitHub (`Unity_InventoryManager-<versao>.zip`).
2. Extrai o ZIP. Fica uma pasta `Unity_InventoryManager`.
3. Arrasta essa pasta para a pasta **Assets** do teu projeto Unity (na janela Project).

Pronto: nao ha mais nada para importar nem configurar. Funciona com o input antigo, o novo ou os dois, e nao precisa de TextMeshPro. Requisitos: Unity 6 e o pacote Unity UI (vem em todos os templates).

**Atualizar:** apaga a pasta `Unity_InventoryManager` e arrasta a nova. Guarda os teus itens e cenas **fora** dessa pasta (por exemplo em `Assets/Items`), senao perdem-se ao atualizar.

**Alternativas (para quem usa o Package Manager):** *Window > Package Manager > + > Add package from disk...* e escolher o `package.json`; ou *Add package from git URL* com `https://github.com/<user>/<repo>.git?path=/Packages/unity.guibot.inventorymanager`. Nao instales das duas maneiras ao mesmo tempo.

## Configurar em 3 passos

1. **Item:** Assets > Create > Inventory > Item. Preenche o nome e arrasta um sprite para `Icon`.
2. **Jogador:** seleciona o jogador na Hierarchy, botao direito > **Inventory > Make Selected the Player**. Adiciona o inventario, os colliders necessarios e a UI.
3. **Pickup:** botao direito no item (janela Project) > **Inventory > Create Pickup 2D in Scene** (ou 3D). Move-o para onde quiseres e carrega em Play.

## Cenas de demo

Menu **Tools > Inventory > Create Demo Scenes** cria em `Assets/InventoryDemo/`:

| Cena | Para que serve |
|------|----------------|
| `Demo2D`, `Demo3D` | So codigo. Um GameObject vazio com o script `Demo2D`/`Demo3D`; ao carregar Play, jogador, itens e pickups sao criados em runtime. No editor a cena aparece vazia. |
| `Finetuning2D`, `Finetuning3D` | Cena montada no editor: jogador, pickups (prefabs) e itens (`ItemData`) ja la estao, visiveis e editaveis. Troca os sprites/modelos, move os pickups, ajusta quantidades. O `InventoryManualSetup` liga a UI e a hotbar. |

## Controlos

| Entrada | Acao |
|---------|------|
| WASD / setas | mover (so nas demos) |
| I | abrir/fechar a grelha |
| 1-9 ou clique esquerdo num slot | selecionar o slot (moldura amarela; ao inicio nenhum) |
| E | usar o item selecionado |
| G | largar o item selecionado (uma unidade) |
| Rato por cima de um slot | mostra o tooltip (nome e descricao) |
| Arrastar um slot para outro | move, junta pilhas iguais ou troca |
| Botao direito num slot | usa o item desse slot |

E e G agem sempre no slot selecionado, o da moldura amarela.

## Configuracao

- **Capacidade:** no `InventoryHolder`, `Columns` x `Rows` (por defeito 4 x 3 = 12 slots). O inventario nao cresce sozinho; quando enche, os pickups ficam no chao.
- **Onde aparece:** no `InventoryUI` (ou no `InventoryManualSetup` da cena Finetuning), `Hotbar Anchor` e `Grid Anchor`: BottomCenter, TopCenter, TopLeft, TopRight, BottomLeft, BottomRight ou Center. `Margin` e a distancia ao bordo; `Hotbar Size` e o numero de slots da hotbar (0 = sem hotbar).
- **Usar um item:** o `ItemData` tem `Consume On Use`. Quando o jogador usa um item, o `InventoryHolder` chama o evento `On Item Used` (Inspector) e o evento C# `OnItemUsed`; liga ai o que o item faz.
- **Largar:** `InventoryDropper` (adicionado sozinho pela UI) cria o pickup a frente do jogador. Se o `ItemData` tiver `World Prefab`, usa esse prefab; senao uma esfera (3D) ou um quadrado (2D), com o `Icon` do item se existir.
- **Descricao:** o campo `Description` do `ItemData` aparece no tooltip.

## Input

`InventoryInput` e o unico sitio que toca numa API de input. Funciona com Active Input Handling em Input System Package (New), Input Manager (Old) ou Both.

## Guardar (opcional, nao ligado)

O inventario nasce vazio e perde-se quando o Play termina; as demos nao guardam nada. A classe `InventorySave` existe para quem quiser: `InventorySave.Save(inventory)` escreve `inventory.json` em `Application.persistentDataPath`, e `InventorySave.Load(inventory, database)` repoe-o (precisa de um `ItemDatabase`, Assets > Create > Inventory > Item Database, com todos os itens).
