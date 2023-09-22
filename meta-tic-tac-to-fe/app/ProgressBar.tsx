import {useGameStore} from '@/app/game-store';
import {useProgressStore} from '@/app/progress-store';
import {useEffect} from 'react';

export default function ProgressBar() {
  const player = useGameStore(state => state.activePlayer);
  const {progress, tick, reset} = useProgressStore();

  /*useEffect(() => {
    const interval = setInterval(() => tick(), 50);
    return () => {
      clearInterval(interval);
    };
  },  []);*/

  useEffect(() => {
    reset();
  }, [player]);

  return (
    <div className={'relative bg-gray-200 mx-2 rounded-lg h-full'} style={{width: 12}}>
      <div className={'absolute bottom-0 rounded-lg bg-red-300 z-10'} style={{width: 12, height: `${progress}%`}}></div>
    </div>
  );
};
