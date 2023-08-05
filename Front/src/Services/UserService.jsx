import {axiosClient} from "./AxiosService";

export const Login = async (requestBody) => {
    return await axiosClient.post(
        `${process.env.REACT_APP_API_ENDPOINT}/login`, requestBody
    );
};

export const Register = async (requestBody) => {
    return await axiosClient.post(
        `${process.env.REACT_APP_API_ENDPOINT}/register`, requestBody
    );
};

export const GetUserInfoAction = async (requestBody) => {
    return await axiosClient.get(
        `${process.env.REACT_APP_API_ENDPOINT}/user`
    );
};

export const UpdateUser = async (requestBody) => {
    return await axiosClient.put(
        `${process.env.REACT_APP_API_ENDPOINT}/update-profile`, requestBody
    );
};

export const UploadImage = async (requestBody) => {
    return await axiosClient.put(
        `${process.env.REACT_APP_API_ENDPOINT}/upload-image`, requestBody
    );
};

export const GetImage = async (requestBody) => {
    return await axiosClient.get(
        `${process.env.REACT_APP_API_ENDPOINT}/get-image`
    );
};

export const ChangePassword = async (requestBody) => {
    return await axiosClient.post(
        `${process.env.REACT_APP_API_ENDPOINT}/change-password`, requestBody
    );
};

export const GoogleLogin = async (requestBody) => {
    return await axiosClient.post(
        `${process.env.REACT_APP_API_ENDPOINT}/external-login`, requestBody
    );
};

export const GetInactiveSellers = async () => {
    return await axiosClient.get(
        `${process.env.REACT_APP_API_ENDPOINT}/unactivated-sellers`
    );
};

export const GetActiveSellers = async () => {
    return await axiosClient.get(
        `${process.env.REACT_APP_API_ENDPOINT}/activated-sellers`
    );
};

export const ApproveSeller = async (requestBody) => {
    return await axiosClient.put(
        `${process.env.REACT_APP_API_ENDPOINT}/activate-user`, requestBody
    );
};