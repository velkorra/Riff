import { useEffect, useRef } from 'react';
import * as signalR from '@microsoft/signalr';

export const useSignalR = (roomId: string, handlers: any) => {
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`/hub`)
      .withAutomaticReconnect()
      .build();

    connection.start()
      .then(() => {
        console.log('SignalR Connected');
        connection.invoke('JoinRoom', roomId);
      })
      .catch(err => console.error('SignalR Connection Error: ', err));

    connection.on('TrackAdded', (data) => handlers.onTrackAdded && handlers.onTrackAdded(data));
    connection.on('PlaybackChanged', (data) => handlers.onPlaybackChanged && handlers.onPlaybackChanged(data));
    connection.on('ScoreUpdated', (data) => handlers.onScoreUpdated && handlers.onScoreUpdated(data));
    connection.on('TrackRemoved', (data) => handlers.onTrackRemoved && handlers.onTrackRemoved(data));

    connectionRef.current = connection;

    return () => {
      connection.stop();
    };
  }, [roomId]);

  return connectionRef.current;
};