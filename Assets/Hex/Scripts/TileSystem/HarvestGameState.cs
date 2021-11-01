using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData {
    public bool scoreDelta = false;
    public PlayerInfo[] playerInfo = { };

    public int playerCount = 4;
    public int activePlayer = 0;
}

[ExecuteInEditMode]
public class HarvestGameState : MonoBehaviour
{
    public TileSystem tileSystem;

    public GameStatus gameStatus;
    public PlayerData playerData;
    public PlayerInfo[] playerInfo = { };

    public int playerCount = 4;

    public GameObject uiIngame;
    public GameObject uiMenu;
    public Text uiPlayerCountDisplay;

    public List<TileInfo> prefabTileTypes;
    public List<EntityInfo> prefabEntityTypes;

    private void OnValidate() {
        gameInit();
    }

    // ===========================

    public void gameInit() {
        playerData = new PlayerData();
        playerData.playerInfo = playerInfo;
        playerData.playerCount = playerCount;
        playerData.activePlayer = 0;
        for(int p = 0; p < playerData.playerCount; ++p) {
            playerInfo[p].scores = new List<int>(new int[9]);
            playerInfo[p].ownedTiles = new List<HexTile>();
            playerInfo[p].ownedEntities = new List<HexEntity>();
        }
        tileSystem.playerData = playerData;
        gameStatus.playerData = playerData;
        gameStatus.setPlayerCount(playerCount);
        gameStatus.repaint();
    }

    public void uinavIncreasePlayers() {
        playerCount += 1;
        if(playerCount > 4)
            playerCount = 4;
        gameStatus.setPlayerCount(playerCount);
        uiPlayerCountDisplay.text = ""+playerCount;
    }
    public void uinavDecreasePlayers() {
        playerCount -= 1;
        if(playerCount < 1)
            playerCount = 1;
        gameStatus.setPlayerCount(playerCount);
        uiPlayerCountDisplay.text = ""+playerCount;
    }
    public void uinavStartGame() {
        gameInit();
        uiIngame.SetActive(true);
        uiMenu.SetActive(false);
    }
    public void uinavEnterMenu() {
        uiIngame.SetActive(false);
        uiMenu.SetActive(true);
    }

    public void nextTurn() {
        playerData.activePlayer = (playerData.activePlayer + 1)%playerCount;
        gameStatus.playerData.activePlayer = playerData.activePlayer;
        gameStatus.repaint();
    }

    public void gameUpdate() {
        if(playerData != null) {
            if(playerData.scoreDelta) {
                playerData.scoreDelta = false;
                gameStatus.repaint();
            }
        }
        if(tileSystem) {
            if(tileSystem.gameUpdate()) {
                // a tile/entity have been placed
                TileSystem.HistoryRecord last = tileSystem.getLatestHistoryItem();
                if(last.isTile) {
                    HexTile tile = last.tile;
                    tile.playerOwner = playerData.activePlayer;
                    //tile.setColor(playerInfo[activePlayer].playerColor);
                    playerInfo[playerData.activePlayer].ownedTiles.Add(tile);
                } else { // isEntity
                    HexEntity entity = last.entity;
                    entity.playerOwner = playerData.activePlayer;
                    entity.setColor(playerInfo[playerData.activePlayer].playerColor);
                    entity.generateJobs(tileSystem);
                    playerInfo[playerData.activePlayer].ownedEntities.Add(entity);
                }
                tileSystem.updateJobs();
                nextTurn();
            }
        }
    }

    // =============================
    public void clearAll() {
        if(tileSystem) {
            tileSystem.clearAll();
        }
    }
    public void restartGame() {
        clearAll();
        gameInit();
    }
    

    public void spawnProtoTile(int index) {
        if(prefabTileTypes.Count < index) {
            Debug.LogError("Tile Info Array index out of bounds!");
            return;
        }

        TileInfo ti = prefabTileTypes[index];
        if(tileSystem) {
            tileSystem.spawnProtoTile(ti);
        }
    }

    public void spawnProtoEntity(int index) {
        if(prefabEntityTypes.Count < index) {
            Debug.LogError("Entity Info Array index out of bounds!");
            return;
        }

        EntityInfo ei = prefabEntityTypes[index];
        if(tileSystem) {
            tileSystem.spawnProtoTile(ei);
        }
    }

    public void spawnProtoTile(TileInfo ti) {
        if(tileSystem) {
            tileSystem.spawnProtoTile(ti);
        }
    }

    public void spawnProtoEntity(EntityInfo ei) {
        if(tileSystem) {
            tileSystem.spawnProtoTile(ei);
        }
    }
}
