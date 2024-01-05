import useSWR from 'swr';

const fetcher = (url:string) => fetch(url).then(res => res.json())

export function useSubscription(companyName:string) {
    const { data, error, isLoading } = useSWR(`/api/v1/subscriptions/${companyName}`, fetcher)

    return {
        subscription: data,
        isLoading,
        isError: error
    }
}


