import {useProgressStore} from '@/app/progress-store';
import {useGameStore} from '@/app/game-store';

export default function EndScreen({onNewGame}: {onNewGame?: () => void}) {
  const progress = useProgressStore(state => state.progress);
  const winner = useGameStore(state => state.winner);
  const newGame = useGameStore(state => state.newGame);
  const resetProgress = useProgressStore(state => state.reset);

  if (!winner) {
    return null;
  }

  const handleClick = () => {
    newGame();
    resetProgress();

    if (onNewGame) {
      onNewGame();
    }
  };

  return (
    <div className={
      'absolute left-0 top-0 bg-white/60 z-30 backdrop-blur w-screen h-screen flex justify-center items-center'
    }>
      <div className={'flex flex-col items-center'}>
        <h1 className={'text-6xl italic -rotate-6 font-bold animate-pulse'}>Spieler {winner} gewonnen!</h1>
        <button className={'mt-28 text-3xl bg-white rounded-xl py-6 px-12 w-fit hover:animate-ping'}
                onClick={handleClick}>
          Neues Spiel
        </button>
      </div>
      <div className={'absolute -z-10 blur-3xl bg-gradient-to-br from-cyan-400 to-amber-400 rounded-full'}
           style={{width: 400, height: 400}}>
      </div>
    </div>
  );
}
