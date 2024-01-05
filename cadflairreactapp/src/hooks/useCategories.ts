import useSWR from 'swr';

const fetcher = (url:string) => fetch(url).then(res => res.json())

//this hook is not yet working
export function useCategories(subscriptionId:number) {
    const { data, error, isLoading } = useSWR(`/api/v1/subscriptions/${subscriptionId}/categories/`, fetcher)

    return {
        categories: data,
        categoriesLoading: isLoading,
        isError: error
    }
}


