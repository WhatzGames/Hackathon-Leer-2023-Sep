import {create} from 'zustand';

type ProgressStore = {
  progress: number;
  tick: () => number;
  reset: () => void;
}

export const useProgressStore = create<ProgressStore>((set, get) => ({
  progress: 100,
  tick() {
    let newValue = get().progress - 1;
    if (newValue < 0) {
      newValue = 0;
    }

    set((state) => ({
      ...state,
      progress: newValue
    }));

    return newValue;
  },
  reset() {
    set((state) => ({
      ...state,
      progress: 100
    }))
  }
}));
