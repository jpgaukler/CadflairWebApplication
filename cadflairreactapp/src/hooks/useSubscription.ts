import useSWR from 'swr';
import Subscription from '../interfaces/Subscription.interface';

const fetcher = (url:string) => fetch(url).then(res => res.json())

export function useSubscription(companyName:string) {
    const { data, error, isLoading } = useSWR<Subscription>(`/api/v1/subscriptions/${companyName}`, fetcher)

    return {
        subscription: data,
        isLoading,
        isError: error
    }
}


