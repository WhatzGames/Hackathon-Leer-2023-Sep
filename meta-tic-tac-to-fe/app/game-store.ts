'use client';

import {create} from 'zustand';
import {v4 as randomUUID} from 'uuid';
import {Board, Game, PlayedGame, Player, PlayerSymbol} from '@/app/types';
import {Simulate} from 'react-dom/test-utils';
import play = Simulate.play;

type GameState = {
  game: Game;
  players: Player[];
  playedGame: PlayedGame | null;
  nextStepIndex: number;
  activePlayer: string;
  activeBoard: number | null;
  overview: (PlayerSymbol | null)[];
}

type GameActions = {
  load: (playedGame: PlayedGame) => void;
  nextStep: () => void;
  setField: (board: number, field: number) => void;
  switchPlayer: () => void;
  newGame: () => void;
}

const checkBoard = ({fields}: Board): PlayerSymbol | null => {
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
    {id: '1', symbol: 'X', score: 0},
    {id: '2', symbol: 'O', score: 0},
  ];

  return {
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
    playedGame: null,
    nextStepIndex: 0,
    players,
    activePlayer: players.at(0)!.id,
    activeBoard: null,
    overview: [null, null, null, null, null, null, null, null, null],
  }
}

export const useGameStore = create<GameState & GameActions>((set, get) => ({
  ...initialState(),
  load(playedGame: PlayedGame) {
    set((state) => ({
      ...state,
      players: playedGame.players,
      activePlayer: playedGame.players.at(0)!.id,
      playedGame
    }));
  },
  nextStep() {
    const playedGame = get().playedGame;
    if (!playedGame) {
      return;
    }

    const stepIndex = get().nextStepIndex;
    const step = playedGame.log[stepIndex];

    if (step) {
      get().setField(...step.move);
    }
  },
  setField(board: number, field: number) {
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

    const winner = checkBoard(game[board]);
    if (winner) {
      overview[board] = winner;
    }

    // Set new active to last field, otherwise allow random choice
    activeBoard = overview[field] != null ? null : field;

    set((state) => ({
      ...state,
      activePlayer: activePlayerIndex === 0 ? players[1].id : players[0].id,
      activeBoard,
      overview: overview,
      game,
      nextStepIndex: get().nextStepIndex + 1
    }));
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
  }
}));
