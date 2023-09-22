import {PlayedGame} from '@/app/types';

export default async function HistoryPage() {
  const playedGames: PlayedGame[] = await fetch('http://localhost:3000/api/history')
    .then(res => res.json());

  return (
    <main id={'history-page'}>
      <h1 className={'text-3xl'}>Historie</h1>
      <select name="" id="" className={'border border-slate'}>
        {playedGames.map(p => <option key={p.id} value={p.id}>{p.id}</option>)}
      </select>
    </main>
  );
}
