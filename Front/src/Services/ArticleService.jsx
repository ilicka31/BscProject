import {axiosClient} from "./AxiosService";

export const GetAllArticles = async () => {
    return await axiosClient.get(
        `${process.env.REACT_APP_API_ENDPOINT}/get-all`
    );
};

export const NewArticle = async (requestBody) => {
    console.log(requestBody);
    return await axiosClient.put(
        `${process.env.REACT_APP_API_ENDPOINT}/new-article`, requestBody
    );
};

export const UpdateArticle = async (requestBody) => {
    return await axiosClient.put(
        `${process.env.REACT_APP_API_ENDPOINT}/update-article`, requestBody
    );
};

export const DeleteArticle = async (requestBody) => {
    return await axiosClient.put(
        `${process.env.REACT_APP_API_ENDPOINT}/delete`, requestBody
    );
};

export const GetSellerArticles = async () => {
    return await axiosClient.get(
        `${process.env.REACT_APP_API_ENDPOINT}/seller-get`
    );
};

export const UploadArticleImage = async (requestBody) => {
    return await axiosClient.put(
        `${process.env.REACT_APP_API_ENDPOINT}/upload-product-image`, requestBody
    );
};

export const GetArticleImage = async (requestBody) => {
    return await axiosClient.get(
        `${process.env.REACT_APP_API_ENDPOINT}/get-product-image`, requestBody
    );
};