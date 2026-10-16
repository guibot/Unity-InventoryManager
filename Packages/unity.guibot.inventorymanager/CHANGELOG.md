# Changelog

## 0.2.1
- Corrigido: sprites 2D criados pelo package (jogador, pickups, itens largados, icones) ficavam invisiveis em alguns projetos. Passam a usar o material `Sprites-Default`, que desenha em qualquer renderer.
- Nota: as cenas de demo ja criadas mantem o material antigo. Apaga a pasta `Assets/InventoryDemo` e volta a correr *Tools > Inventory > Create Demo Scenes*.

## 0.2.0
- Largar itens (G), usar itens (E ou botao direito), arrastar para trocar slots, tooltip.
- Posicao da hotbar e da grelha configuravel; grelha de `Columns x Rows` no `InventoryHolder`.
- Sem TextMeshPro: usa o `Text` normal do Unity, nao ha nada para importar.
- Sem dependencia do Input System: funciona com o input antigo, o novo, ou os dois.
- Menus de um clique: GameObject > Inventory > Make Selected the Player; botao direito num Item > Inventory > Create Pickup 2D/3D in Scene.

## 0.1.0
- Primeira versao: `ItemData`, `Inventory`, pickups 2D e 3D, UI com grelha e hotbar.
