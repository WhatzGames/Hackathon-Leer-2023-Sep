'use client';

import {create} from 'zustand';
import {v4 as randomUUID} from 'uuid';
import {Field, Game, Player, PlayerSymbol} from '@/app/types';

type GameState = {
  game: Game;
  wait: boolean;
  players: Player[];
  activePlayer: string;
  winner: PlayerSymbol | null;
  activeBoard: number | null;
  overview: Field[];
}

type GameActions = {
  setField: (board: number, field: number) => void;
  setPlayers: (players: Player[]) => void;
  switchPlayer: () => void;
  newGame: () => void;
  endGame: () => void;
}

const checkBoard = (fields: Field[]): PlayerSymbol | null => {
  const combinations = [
    [0, 1, 2],
    [3, 4, 5],
    [6, 7, 8],
    [0, 3, 6],
    [1, 4, 7],
    [2, 5, 8],
    [0, 4, 8],
    [2, 4, 6],
  ];

  for (const combination of combinations) {
    if (!fields[combination[0]] || !fields[combination[1]] || !fields[combination[2]]) {
      continue;
    }

    if (fields[combination[0]] === fields[combination[1]] && fields[combination[1]] === fields[combination[2]]) {
      return fields[combination[0]];
    }
  }

  return null;
};

const initialState = (): GameState => {
  const players: Player[] = [
    {id: 'Spieler 1', symbol: 'X', score: 0},
    {id: 'Spieler 2', symbol: 'O', score: 0},
  ];

  return {
    wait: false,
    game: [
      {id: randomUUID(), fields: [null, null, null, null, null, null, null, null, null]},
      {id: randomUUID(), fields: [null, null, null, null, null, null, null, null, null]},
      {id: randomUUID(), fields: [null, null, null, null, null, null, null, null, null]},
      {id: randomUUID(), fields: [null, null, null, null, null, null, null, null, null]},
      {id: randomUUID(), fields: [null, null, null, null, null, null, null, null, null]},
      {id: randomUUID(), fields: [null, null, null, null, null, null, null, null, null]},
      {id: randomUUID(), fields: [null, null, null, null, null, null, null, null, null]},
      {id: randomUUID(), fields: [null, null, null, null, null, null, null, null, null]},
      {id: randomUUID(), fields: [null, null, null, null, null, null, null, null, null]},
    ],
    winner: null,
    players,
    activePlayer: players.at(0)!.id,
    activeBoard: null,
    overview: [null, null, null, null, null, null, null, null, null],
  };
};

export const useGameStore = create<GameState & GameActions>((set, get) => ({
  ...initialState(),
  setField(board: number, field: number) {
    if (get().wait) {
      return;
    }

    // board not active - not allowed
    let activeBoard = get().activeBoard;
    if (activeBoard !== null && board !== activeBoard) {
      return;
    }

    // Board finished - not allowed
    const overview = get().overview;
    if (overview[board] !== null) {
      return;
    }

    // Field set - not allowed
    const game = structuredClone(get().game);
    if (game[board].fields[field] != null) {
      return;
    }

    // Set field to player
    const players = get().players;
    const activePlayerIndex = players.findIndex(player => player.id === get().activePlayer);
    game[board].fields[field] = players[activePlayerIndex].symbol;

    const boardWinner = checkBoard(game[board].fields);
    if (boardWinner) {
      overview[board] = boardWinner;
    }

    const winner = checkBoard(overview);
    let gameWinner = get().winner;
    if (winner) {
      gameWinner = winner;
    }

    // Set new active to last field, otherwise allow random choice
    activeBoard = overview[field] != null ? null : field;

    set((state) => ({
      ...state,
      wait: true,
      overview: overview,
      game,
      winner: gameWinner
    }));

    setTimeout(() => {
      set((state) => ({
        ...state,
        wait: false,
        activePlayer: activePlayerIndex === 0 ? players[1].id : players[0].id,
        activeBoard
      }));
    }, 500);
  },
  setPlayers(players: Player[]) {
    set((state) => ({
      ...state,
      players,
      activePlayer: players.at(0)!.id
    }))
  },
  switchPlayer() {
    set((state) => ({
      ...state,
      activePlayer: state.activePlayer === 'X' ? 'O' : 'X',
    }));
  },
  newGame() {
    set((state) => ({
      ...state,
      ...initialState(),
    }));
  },
  endGame() {
    const activePlayer = get().activePlayer;
    const winner = get().players.find(player => player.id !== activePlayer);

    if (!winner) {
      throw new Error('Could not determine winner.');
    }

    set((state) => ({
      ...state,
      winner: winner.symbol
    }));
  }
}));
